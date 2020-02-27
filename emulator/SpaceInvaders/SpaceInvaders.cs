
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        #region Debugging etc

        /**
         * Enables debugging statistics and features.
         */
        public bool Debug { get; set; } = false;

        /**
         * When Debug=true, the program will break at these addresses and allow the user to perform
         * interactive debugging via the console.
         */
        public List<UInt16> BreakAtAddresses { get; set; }

        /**
         * Indicates if we're stingle stepping through opcodes/instructions using the interactive
         * debugger when Debug=true.
         */
        private bool _singleStepping = false;

        /**
         * For use by the interactive debugger when Debug=true. If true, indicates that the disassembly
         * should be annotated with the values in the Annotations dictionary. If false, the diassembler
         * will annotate each line with a pseudocode comment instead.
         */
        private bool _showAnnotatedDisassembly = false;

        /**
         * The annotations to be used when Debug=true and _showAnnotatedDisassembly=true. It is a map
         * of memory addresses to string annotation values.
         */
        public Dictionary<UInt16, String> Annotations { get; set; }

        // For Debugging; updated when Debug = true
        private int _totalCycles = 0;
        private int _totalSteps = 0;
        private List<UInt16> _addressHistory = new List<UInt16>();
        private static readonly int MAX_ADDRESS_HISTORY = 100;

        #endregion

        public delegate void RenderEvent(RenderEventArgs e);
        public event RenderEvent OnRender;
        private RenderEventArgs _renderEventArgs;

        private Thread _thread;
        private bool _cancelled = false;

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

        // To keep the emulated CPU from running too fast, we use a stopwatch and count cycles.
        private Stopwatch _cpuStopWatch = new Stopwatch();
        private int _cycleCount = 0;

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

            _cancelled = false;
            _thread = new Thread(new ThreadStart(HardwareLoop));
            _thread.Name = "Emulator: Hardware Loop";
            _thread.Start();
        }

        public void Stop()
        {
            if (_thread == null)
                throw new Exception("Emulator cannot be stopped because it wasn't running.");

            _cancelled = true;
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
        public void PrintMemory(UInt16 address, bool annotate = false, int beforeCount = 10, int afterCount = 10)
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

                // If we're showing annotations, then don't show the pseudocode.
                var emitPseudocode = !_showAnnotatedDisassembly;

                // Disasemble the opcode and print it.
                var instruction = Disassembler.Disassemble(_cpu.Memory, addressIndex, out int instructionLength, true, emitPseudocode);
                output.Append(instruction);

                // If we're showing annotations, attempt to look up the annotation for this address.
                if (_showAnnotatedDisassembly && Annotations != null)
                {
                    var annotation = Annotations.ContainsKey(addressIndex) ? Annotations[addressIndex] : null;
                    output.Append("\t\t; ");
                    output.Append(annotation == null ? "???" : annotation);
                }

                output.AppendLine();

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
                    output.AppendLine($"\t{addressFormatted}\t(D8: {dataFormatted})");
                    i++;
                }
            }

            Console.WriteLine(output.ToString());
        }

        /**
         * Used to print the disassembly of the memory locations around where the program counter is pointing.
         * Useful when a debugger is attached.
         */
        public void PrintCurrentExecution(bool annotate = false, int beforeCount = 10, int afterCount = 10)
        {
            PrintMemory(_cpu.ProgramCounter, annotate, beforeCount, afterCount);
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
            _cpuStopWatch.Start();
            _cycleCount = 0;

            while (!_cancelled)
            {
                #region Interactive Debugging

                if (Debug)
                {
                    // Record the current address.

                    _addressHistory.Add(_cpu.ProgramCounter);

                    if (_addressHistory.Count >= MAX_ADDRESS_HISTORY)
                        _addressHistory.RemoveAt(0);

                    // See if we need to break based on a given address.
                    if (BreakAtAddresses != null && BreakAtAddresses.Contains(_cpu.ProgramCounter))
                        _singleStepping = true;

                    // If we need to break, print out the CPU state and wait for a keypress.
                    if (_singleStepping)
                    {
                        // Print debug information and wait for user input via the console (key press).
                        while (true)
                        {
                            Console.WriteLine("-------------------------------------------------------------------");
                            Console.WriteLine($" Total Steps/Opcodes: {_totalSteps}\tCPU Cycles: {_totalCycles}");
                            Console.WriteLine();
                            _cpu.PrintDebugSummary();
                            Console.WriteLine();
                            PrintCurrentExecution(_showAnnotatedDisassembly);
                            Console.WriteLine();

                            Console.WriteLine(" F5 = Continue    F10 = Step    F11 = Toggle Annotated Disassembly    F12 = Print List 10 Opcodes");
                            var key = Console.ReadKey(); // Blocking

                            // Handle console input.
                            if (key.Key == ConsoleKey.F5) // Continue
                            {
                                _singleStepping = false;
                                break; // Break out of input loop
                            }
                            else if (key.Key == ConsoleKey.F10) // Step
                            {
                                break; // Break out of input loop
                            }
                            else if (key.Key == ConsoleKey.F11) // Toggle Annotated Disassembly
                            {
                                _showAnnotatedDisassembly = !_showAnnotatedDisassembly;
                            }
                            else if (key.Key == ConsoleKey.F12) // Print List 10 Opcodes
                            {
                                Console.WriteLine("-------------------------------------------------------------------");
                                Console.WriteLine();
                                Console.WriteLine(" Last 10 Opcodes: ");
                                Console.WriteLine();
                                PrintRecentInstructions(10);
                            }
                        }
                    }
                }

                #endregion

                // Step the CPU to execute the next instruction.
                var cycles = _cpu.Step();

                // Keep track of the number of cycles to see if we need to throttle the CPU.
                _cycleCount += cycles;

                #region Debugging Stats

                // Keep track of the total number of steps (instructions) and cycles ellapsed.
                if (Debug)
                {
                    _totalSteps++;
                    _totalCycles += cycles;

                    // Used to slow down the emulation to watch the renderer.
                    // if (_totalCycles % 1000 == 0)
                    //     System.Threading.Thread.Sleep(10);
                }

                #endregion

                // Throttle the CPU emulation if needed.
                if (_cycleCount >= (CPU_MHZ/60))
                {
                    _cpuStopWatch.Stop();

                    if (_cpuStopWatch.Elapsed.TotalMilliseconds < 16.6)
                    {
                        var sleepForMs = 16.6 - _cpuStopWatch.Elapsed.TotalMilliseconds;

                        if (sleepForMs >= 1)
                            System.Threading.Thread.Sleep((int)sleepForMs);
                    }

                    _cycleCount = 0;
                    _cpuStopWatch.Restart();
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

            _cpu = null;
            _thread = null;
        }
    }
}
