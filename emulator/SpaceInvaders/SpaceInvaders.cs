
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

        private Thread _thread;
        private CPU _cpu;

        // The game's video hardware generates runs at 60hz. It generates two interrupts @ 60hz. Interrupt
        // #1 the middle of a frame and interrupt #2 at the end (vblank). To simulate this, we'll calculate
        // the number of cycles we're expecting between each of these interrupts. While this is not entirely
        // accurate, it is close enough for the game to run as expected.
        private double _cyclesPerInterrupt = Math.Floor(Convert.ToDouble(CPU_MHZ / 60 / 2));
        private int _cyclesSinceLastInterrupt = 0;
        private Interrupt _nextInterrupt;

        // TODO: Implement I/O ports
        // TODO: Implement shift register port
        // TODO: Implement audio event emitter
        // TODO: Implement framebuffer emitter
        // TODO: Implement input handler

        public void Start(byte[] rom)
        {
            if (_thread != null)
                throw new Exception("Emulator cannot be started because it was already running.");

            _cyclesSinceLastInterrupt = 0;
            _nextInterrupt = Interrupt.One;

            _cpu = new CPU();
            _cpu.LoadRom(rom);

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
