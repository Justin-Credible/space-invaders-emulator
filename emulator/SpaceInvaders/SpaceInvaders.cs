
using System;
using System.Threading;

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

            _thread = new Thread(new ThreadStart(Loop));
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

        private void Loop()
        {
            var cycles = _cpu.Step();

            _cyclesSinceLastInterrupt += cycles;

            if (_cyclesSinceLastInterrupt >= _cyclesPerInterrupt)
            {
                if (_cpu.InterruptsEnabled)
                {
                    _cpu.StepInterrupt(_nextInterrupt);

                    if (_nextInterrupt == Interrupt.One)
                        _nextInterrupt = Interrupt.Two; // End of screen (vblank).
                    else if (_nextInterrupt == Interrupt.Two)
                        _nextInterrupt = Interrupt.One; // Middle of screen.
                    else
                        throw new Exception($"Unexpected next interrupt: {_nextInterrupt}.");
                }

                _cyclesSinceLastInterrupt = 0;
            }
        }
    }
}
