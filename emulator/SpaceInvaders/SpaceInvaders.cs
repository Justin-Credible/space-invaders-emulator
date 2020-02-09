
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using JustinCredible.I8080Disassembler;
using JustinCredible.Intel8080;

namespace JustinCredible.SIEmulator
{
    public class SpaceInvaders
    {
        // The Intel 8080 for Space Invadiers is clocked at 2MHz.
        private const int CPU_MHZ = 2000000;

        // While the resolution is indeed 256x224, note that the monitor for this
        // game is portrait, not landscape. It is rotated -90 degrees (counterclockwise)
        // in the cabinet and therefore the resolution viewable to the user will be 224x256.
        // The framebuffer will need to be rotated before it is displayed to the end user.
        public const int RESOLUTION_WIDTH = 256;
        public const int RESOLUTION_HEIGHT = 224;

        // The frame buffer is 256 x 224, which is 57,344 pixels. Since the display is only
        // black and white, we only need one bit per pixel. Therefore we need 57,344 / 8
        // => 7,168 bytes or 7 KB for the frame buffer. This is pulled from the video RAM
        // portion which is at $2400-$3fff.
        private const int FRAME_BUFFER_SIZE = 1024 * 7;

        // The configuration of the Intel 8080 CPU specifically for Space Invaders.
        private static readonly CPUConfig _cpuConfig = new CPUConfig()
        {
            // 16 KB of RAM
            MemorySize = 16 * 1024,

            /**
             * This is the memory layout specific to Space Invaders:
             * 
             * ROM (8K)
             * $0000-$07ff:  invaders.h
             * $0800-$0fff:  invaders.g
             * $1000-$17ff:  invaders.f
             * $1800-$1fff:  invaders.e
             * 
             * RAM (8K)
             * $2000-$23ff:  work RAM (1K)
             * $2400-$3fff:  video RAM (7K)
             * 
             * $4000-:       RAM mirror
             * 
             * TODO: Allow writes to 0x4000 - 0x6000 (RAM mirror)?
             */
            WriteableMemoryStart = 0x2000,
            WriteableMemoryEnd = 0x3FFFF,

            // Interrupts are initially disabled, and will be enabled by the program ROM when ready.
            InterruptsEnabled = false,

            EnableDiagnosticsMode = false,
        };

        public bool Debug { get; set; }

        // For Debugging; updated when Debug = true
        private int _totalCycles = 0;
        private int _totalSteps = 0;
        private List<UInt16> _addressHistory = new List<UInt16>();
        private static readonly int MAX_ADDRESS_HISTORY = 100;

        public delegate void RenderEvent(RenderEventArgs e);
        public event RenderEvent OnRender;
        private RenderEventArgs _renderEventArgs;

        private Thread _thread;

        // Intel 8080
        private CPU _cpu;

        // Dedicated Shift Hardware
        private ShiftRegister _shiftRegister;

        // The game's video hardware generates runs at 60hz. It generates two interrupts @ 60hz. Interrupt
        // #1 the middle of a frame and interrupt #2 at the end (vblank). To simulate this, we'll calculate
        // the number of cycles we're expecting between each of these interrupts. While this is not entirely
        // accurate, it is close enough for the game to run as expected.
        private double _cyclesPerInterrupt = Math.Floor(Convert.ToDouble(CPU_MHZ / 60 / 2));
        private int _cyclesSinceLastInterrupt = 0;
        private Interrupt _nextInterrupt;

        // TODO: Implement I/O ports
        // TODO: Implement audio event emitter
        // TODO: Implement framebuffer emitter
        // TODO: Implement input handler

        public void Start(byte[] rom)
        {
            if (_thread != null)
                throw new Exception("Emulator cannot be started because it was already running.");

            _cyclesSinceLastInterrupt = 0;
            _nextInterrupt = Interrupt.One;

            _cpu = new CPU(_cpuConfig);

            _cpu.OnDeviceRead += CPU_OnDeviceRead;
            _cpu.OnDeviceWrite += CPU_OnDeviceWrite;

            // Map the ROM into the lower 8K of the memory space.
            var memory = new byte[_cpuConfig.MemorySize];
            Array.Copy(rom, memory, rom.Length);

            _cpu.LoadMemory(memory);

            _shiftRegister = new ShiftRegister();

            _renderEventArgs = new RenderEventArgs()
            {
                ShouldRender = false,
                FrameBuffer = new byte[FRAME_BUFFER_SIZE],
            };

            _thread = new Thread(new ThreadStart(HardwareLoop));
            _thread.Name = "Emulator: Hardware Loop";
            _thread.Start();
        }

        public void Stop()
        {
            if (_thread == null)
                throw new Exception("Emulator cannot be stopped because it wasn't running.");

            _thread.Abort();
            _cpu = null;
            _thread = null;
        }

        /**
         * Prints last n instructions that were executed up to MAX_ADDRESS_HISTORY.
         * Useful when a debugger is attached. Only works when Debug is true.
         */
        public void PrintRecentInstructions(int count = 10)
        {
            if (!Debug)
                return;

            var output = new StringBuilder();

            if (count > _addressHistory.Count)
                count = _addressHistory.Count;

            var startIndex = _addressHistory.Count - count;

            for (var i = startIndex; i < _addressHistory.Count; i++)
            {
                var address = _addressHistory[i];

                // Edge case for being able to print instruction history when we've jumped outside
                // of the allowable memory locations.
                if (address >= _cpu.Memory.Length)
                {
                    var addressDisplay = String.Format("0x{0:X4}", address);
                    output.AppendLine($"[IndexOutOfRange: {addressDisplay}]");
                    continue;
                }

                var instruction = Disassembler.Disassemble(_cpu.Memory, address, out _, true, true);
                output.AppendLine(instruction);
            }

            Console.WriteLine(output.ToString());
        }

        /**
         * Used to print the disassembly of memory locations before and after the given address.
         * Useful when a debugger is attached.
         */
        public void PrintMemory(UInt16 address, int beforeCount = 10, int afterCount = 10)
        {
            var output = new StringBuilder();

            // Ensure the start and end locations are within range.
            var start = (address - beforeCount < 0) ? 0 : (address - beforeCount);
            var end = (address + afterCount >= _cpu.Memory.Length) ? _cpu.Memory.Length - 1 : (address + afterCount);

            for (var i = start; i < end; i++)
            {
                var addressIndex = (UInt16)i;

                // If this is the current address location, add an arrow pointing to it.
                output.Append(address == addressIndex ? "---->\t" : "\t");

                // Disasemble the opcode and print it.
                var instruction = Disassembler.Disassemble(_cpu.Memory, addressIndex, out int instructionLength, true, true);
                output.AppendLine(instruction);

                // If the opcode is larger than a single byte, we don't want to print subsequent
                // bytes as opcodes, so here we print the next address locations as the byte value
                // in parentheses, and then increment so we can skip disassembly of the data.
                if (instructionLength == 3)
                {
                    var upper = _cpu.Memory[addressIndex + 2] << 8;
                    var lower = _cpu.Memory[addressIndex + 1];
                    var combined = (UInt16)(upper | lower);
                    var dataFormatted = String.Format("0x{0:X4}", combined);
                    var address1Formatted = String.Format("0x{0:X4}", addressIndex+1);
                    var address2Formatted = String.Format("0x{0:X4}", addressIndex+2);
                    output.AppendLine($"\t{address1Formatted}\t(D16: {dataFormatted})");
                    output.AppendLine($"\t{address2Formatted}\t");
                    i += 2;
                }
                else if (instructionLength == 2)
                {
                    var dataFormatted = String.Format("0x{0:X2}", _cpu.Memory[addressIndex+1]);
                    var addressFormatted = String.Format("0x{0:X4}", addressIndex+1);
                    output.AppendLine($"\t{addressFormatted}\t(D8: {dataFormatted}");
                    i++;
                }
            }

            Console.WriteLine(output.ToString());
        }

        /**
         * Used to print the disassembly of the memory locations around where the program counter is pointing.
         * Useful when a debugger is attached.
         */
        public void PrintCurrentExecution(int beforeCount = 10, int afterCount = 10)
        {
            PrintMemory(_cpu.ProgramCounter, beforeCount, afterCount);
        }

        /**
         * Used to handle the CPU's IN instruction; read value from given device ID.
         */
        private byte CPU_OnDeviceRead(int deviceID)
        {
            // http://computerarcheology.com/Arcade/SpaceInvaders/Hardware.html
            switch (deviceID)
            {
                // Inputs - Ports 0-2
                case 0x00:
                case 0x01:
                case 0x02:
                    // TODO
                    return 0x00;

                // Shift Register - Read
                case 0x03:
                    return _shiftRegister.Read();

                default:
                    Console.WriteLine($"WARNING: An IN/Read for port {deviceID} is not implemented.");
                    return 0x00;
            }
        }

        /**
         * Used to handle the CPU's OUT instruction; write value to given device ID.
         */
        private void CPU_OnDeviceWrite(int deviceID, byte data)
        {
            //http://computerarcheology.com/Arcade/SpaceInvaders/Hardware.html
            switch (deviceID)
            {
                // Shift Register - Set Offset
                case 0x02:
                    _shiftRegister.SetOffset(data);
                    break;

                // Shift Register - Write
                case 0x04:
                    _shiftRegister.Write(data);
                    break;

                case 0x03: // Sounds
                case 0x05: // Sounds
                    // TODO
                    break;

                default:
                    Console.WriteLine($"WARNING: An OUT/Write for port {deviceID} (value: {data}) is not implemented.");
                    break;
            }
        }

        /**
         * Handles stepping the CPU to execute instructions as well as firing interrupts.
         */
        private void HardwareLoop()
        {
            while (true)
            {
                // Record the current address.
                if (Debug)
                {
                    _addressHistory.Add(_cpu.ProgramCounter);

                    if (_addressHistory.Count >= MAX_ADDRESS_HISTORY)
                        _addressHistory.RemoveAt(0);
                }

                // Step the CPU to execute the next instruction.
                var cycles = _cpu.Step();

                // Keep track of the total number of steps (instructions) and cycles ellapsed.
                if (Debug)
                {
                    _totalSteps++;
                    _totalCycles += cycles;
                }

                // Keep track of the number of cycles since the last interrupt occurred.
                _cyclesSinceLastInterrupt += cycles;

                // Determine if it's time for the video hardware to fire an interrupt.
                if (_cyclesSinceLastInterrupt >= _cyclesPerInterrupt)
                {
                    // If interrupts are enabled, then handle them, otherwise do nothing.
                    if (_cpu.InterruptsEnabled)
                    {
                        // If we're going to run an interrupt handler, ensure interrupts are disabled.
                        // This ensures we don't interrupt the interrupt handler. The program ROM will
                        // re-enable the interrupts manually.
                        _cpu.InterruptsEnabled = false;

                        // Execute the handler for the interrupt.
                        _cpu.StepInterrupt(_nextInterrupt);

                        // The video hardware alternates between these two interrupts.
                        if (_nextInterrupt == Interrupt.One)
                        {
                            // CRT electron beam is at the middle of the screen (approximately).

                            _nextInterrupt = Interrupt.Two;
                        }
                        else if (_nextInterrupt == Interrupt.Two)
                        {
                            // CRT electron beam reached the end (V-Blank).

                            if (OnRender != null)
                            {
                                Array.Copy(_cpu.Memory, 0x2400, _renderEventArgs.FrameBuffer, 0, FRAME_BUFFER_SIZE);
                                OnRender(_renderEventArgs);
                            }

                            _nextInterrupt = Interrupt.One;
                        }
                        else
                            throw new Exception($"Unexpected next interrupt: {_nextInterrupt}.");
                    }

                    // Reset the count so we can count up again.
                    _cyclesSinceLastInterrupt = 0;
                }
            }
        }
    }
}
