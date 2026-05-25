using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Units;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Foundation.Displays;
using System.Diagnostics;

namespace JustinCredible.SIEmulator.MeadowMCU
{
    public class MeadowApp : App<F7FeatherV2>
    {
        private RgbPwmLed _onboardLed;

        private MicroGraphics _canvas;
        private object _renderLock = new object();
        private bool _isRendering = false;

        private SpaceInvaders _game;

        private int _maxStatCount = 10;
        private int _statCount = 0;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            _onboardLed = new RgbPwmLed(
                Device.Pins.OnboardLedRed,
                Device.Pins.OnboardLedGreen,
                Device.Pins.OnboardLedBlue,
                Meadow.Peripherals.Leds.CommonType.CommonAnode);

            InitializeDisplay();

            return base.Initialize();
        }

        private void InitializeDisplay()
        {
            var frequency = new Frequency(48, Frequency.UnitType.Megahertz);

            var config = new SpiClockConfiguration(frequency, SpiClockConfiguration.Mode.Mode3);

            var spiBus = Device.CreateSpiBus(
                clock: Device.Pins.SCK,
                copi: Device.Pins.COPI,
                cipo: Device.Pins.CIPO,
                config: config);

            var display = new St7789(
                spiBus: spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 240,
                height: 240,
                colorMode: ColorMode.Format16bppRgb565);

            _canvas = new MicroGraphics(display);
            _canvas.Clear(updateDisplay: true);
        }

        public override Task Run()
        {
            Console.WriteLine("Starting MCU application code...");
            _onboardLed.SetColor(Color.Yellow);

            // The VSCode extension currently only has one action: build, deploy, and attach the debugger.
            // Running with a debugger attached has a performance penalty. This block can be uncommented
            // so that after the build/deploy/attach operation, the app immediately stops so that we can
            // deattach and press the reset button to reboot and run the deployed code at full speed.
            // if (Debugger.IsAttached)
            // {
            //     _onboardLed.SetColor(Color.Red);
            //     Console.WriteLine("Debugger attached; sleeping main thread forever.");
            //     Thread.Sleep(Timeout.Infinite);
            //     return base.Run();
            // }

            Console.WriteLine("Reading ROM files...");
            var rom = ReadRomFiles(MeadowOS.FileSystem.DataDirectory);

            Console.WriteLine("Initializing emulator...");
            _game = new SpaceInvaders();
            _game.OnEmulationStopped += SpaceInvaders_OnEmulationStopped;
            _game.OnRender += SpaceInvaders_OnRender;
            _game.OnSound += SpaceInvaders_OnSound;

            _game.OnStats += SpaceInvaders_OnStats;
            _game.StatsEnabled = true;

            _onboardLed.SetColor(Color.Purple);

            // Start the emulation; this occurs in a seperate thread and
            // therefore this call is non-blocking.
            Console.WriteLine("Running emulator...");
            _game.Start(rom);

            _onboardLed.SetColor(Color.Aqua);
            Console.WriteLine("Sleeping main thread forever.");
            Thread.Sleep(Timeout.Infinite);

            return base.Run();
        }

        #region Emulator Event Handlers

        private void SpaceInvaders_OnEmulationStopped()
        {
            Console.WriteLine("Emulator stopped!");
            _onboardLed.SetColor(Color.Purple);
        }

        /**
         * Fired when the emulator has a full frame to be rendered.
         * This should occur at approximately 60hz.
         */
        private void SpaceInvaders_OnRender(RenderEventArgs eventArgs)
        {
            lock(_renderLock)
            {
                if (_isRendering)
                {
                    Console.WriteLine("[WARN] A render operation is already in progress; skipping");
                    return;
                }

                _isRendering = true;
            }

            // Render screen from the updated the frame buffer.
            // NOTE: The electron beam scans from left to right, starting in the upper left corner
            // of the CRT when it is in 4:3 (landscape), which is how the framebuffer is stored.
            // However, since the CRT in the cabinet is rotated left (-90 degrees) to show the game
            // in 3:4 (portrait) we need to perform the rotation of points below by starting in the
            // bottom left corner of the window and drawing upwards, ending on the top right.

            // Clear the screen.
            _canvas.Clear(updateDisplay: false);

            var x = 0;
            var y = SpaceInvaders.RESOLUTION_WIDTH - 1;

            // TODO: Adjust for the 240x240 screen; the top/bottom will need to be chopped by 8 pixels each.
            var frameBuffer = eventArgs.FrameBuffer;

            for (var byteIndex = 0; byteIndex < frameBuffer.Length; byteIndex++)
            {
                var value = frameBuffer[byteIndex];

                for (var bit = 0; bit < 8; bit++)
                {
                    if ((value & (1 << bit)) != 0) // Is bit set?
                    {
                        // The CRT is black/white and the framebuffer is 1-bit per pixel.
                        // A transparent overlay added "colors" to areas of the CRT. These
                        // are the approximate y locations of each area/color of the overlay:

                        if (y >= 182 && y <= 223)
                            _canvas.PenColor = Color.Green; // Player and shields
                        else if (y >= 33 && y <= 55)
                            _canvas.PenColor = Color.Red; // UFO
                        else
                            _canvas.PenColor = Color.White; // Everything else

                        _canvas.DrawPixel(x, y);
                    }

                    y--;

                    if (y == -1)
                    {
                        y = SpaceInvaders.RESOLUTION_WIDTH - 1;
                        x++;
                    }

                    if (x == SpaceInvaders.RESOLUTION_HEIGHT)
                        break;
                }

                if (x == SpaceInvaders.RESOLUTION_HEIGHT)
                    break;
            }

            _canvas.Show();

            lock(_renderLock)
            {
                _isRendering = false;
            }
        }

        /**
         * Fired when the emulator needs to play a sound.
         */
        private void SpaceInvaders_OnSound(SoundEventArgs eventArgs)
        {
            //Console.WriteLine("SpaceInvaders_OnSound fired!");
        }

        /**
         * Fired when the emulator is emitting statistic events.
         */
        private void SpaceInvaders_OnStats(StatsEventArgs eventArgs)
        {
            _statCount++;

            var averageMs = eventArgs.TimeMsToVsyncMeasurements.Sum() / eventArgs.TimeMsToVsyncMeasurements.Count();

            if (averageMs > 16.6)
            {
                Console.WriteLine($"[STATS] Overbudget: Average time to execute to vsync was {averageMs} (> 16.6 ms)");
            }
            else
            {
                Console.WriteLine($"[STATS] Underbudget: Average time to execute to vsync was {averageMs} (< 16.6 ms)");
            }

            if (_statCount >= _maxStatCount)
            {
                Console.WriteLine($"[STATS] Stopping emulator after {_maxStatCount} statistic reports");
                _game.Stop();
            }
        }

        #endregion

        #region Helpers

        private byte[] ReadRomFiles(string directoryPath)
        {
            var hPath = Path.Join(directoryPath, "invaders.h");
            var gPath = Path.Join(directoryPath, "invaders.g");
            var fPath = Path.Join(directoryPath, "invaders.f");
            var ePath = Path.Join(directoryPath, "invaders.e");

            if (!File.Exists(hPath))
                throw new Exception($"Could not locate {hPath}");

            if (!File.Exists(gPath))
                throw new Exception($"Could not locate {gPath}");

            if (!File.Exists(fPath))
                throw new Exception($"Could not locate {fPath}");

            if (!File.Exists(ePath))
                throw new Exception($"Could not locate {ePath}");

            // TODO: Checksums?

            var bytes = new List<byte>();

            bytes.AddRange(File.ReadAllBytes(hPath));
            bytes.AddRange(File.ReadAllBytes(gPath));
            bytes.AddRange(File.ReadAllBytes(fPath));
            bytes.AddRange(File.ReadAllBytes(ePath));

            return bytes.ToArray();
        }

        #endregion
    }
}
