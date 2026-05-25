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

namespace JustinCredible.SIEmulator.MeadowMCU
{
    public class MeadowApp : App<F7FeatherV2>
    {
        private Configuration _configuration;

        private RgbPwmLed _onboardLed;
        private St7789 _display;

        private SpaceInvaders _game;
        private Renderer _renderer;

        private int _statCount = 0;

        public override Task Initialize()
        {
            Resolver.Log.Info("Reading app configuration...");
            _configuration = new Configuration(Settings, warning => Resolver.Log.Warn(warning));

            Resolver.Log.Info("Initialize hardware...");

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

            _display = new St7789(
                spiBus: spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 240,
                height: 240,
                colorMode: ColorMode.Format16bppRgb565);

            _display.Clear(updateDisplay: true);
        }

        public override Task Run()
        {
            Resolver.Log.Info("Starting MCU application code...");
            _onboardLed.SetColor(Color.Yellow);

            // The VSCode extension currently only has one action: build, deploy, and attach the debugger.
            // Running with a debugger attached has a performance penalty. This block can be uncommented
            // so that after the build/deploy/attach operation, the app immediately stops so that we can
            // deattach and press the reset button to reboot and run the deployed code at full speed.
            // if (Debugger.IsAttached)
            // {
            //     _onboardLed.SetColor(Color.Red);
            //     Resolver.Log.Info("Debugger attached; sleeping main thread forever.");
            //     Thread.Sleep(Timeout.Infinite);
            //     return base.Run();
            // }

            Resolver.Log.Info("Reading ROM files...");
            var rom = ReadRomFiles(MeadowOS.FileSystem.DataDirectory);

            Resolver.Log.Info("Initializing emulator...");
            _game = new SpaceInvaders();

            // Wire up event listeners.
            _game.OnEmulationStopped += SpaceInvaders_OnEmulationStopped;
            _game.OnRender += SpaceInvaders_OnRender;
            _game.OnSound += SpaceInvaders_OnSound;
            _game.OnStats += SpaceInvaders_OnStats;

            // Set game options.
            _game.StatsEnabled = _configuration.EnableEmulationStats;
            _game.StartingShips = _configuration.StartingShips;
            _game.ExtraShipAt = _configuration.ExtraShipAt;

            // Initialize the renderer and start it. The renderer will wait for frames to be queued and
            // then render them to the given display in a speparate thread.
            _renderer = new Renderer(
                _display,
                enableRenderMetrics: _configuration.EnableRenderMetrics,
                enableDroppedFrameWarnings: _configuration.EnableDroppedFrameWarnings);
            _renderer.Start();

            _onboardLed.SetColor(Color.Purple);

            // Start the game CPU emulation! This occurs in a seperate thread.
            Resolver.Log.Info("Running emulator...");
            _game.Start(rom);

            // The main thread can be used for other things like responding to event handlers.
            // For now, we'll just sleep it indefinitely.
            _onboardLed.SetColor(Color.Aqua);
            Resolver.Log.Info("Sleeping main thread forever.");
            Thread.Sleep(Timeout.Infinite);

            return base.Run();
        }

        #region Emulator Event Handlers

        private void SpaceInvaders_OnEmulationStopped()
        {
            Resolver.Log.Info("Emulator stopped!");
            _onboardLed.SetColor(Color.Purple);
            _renderer.Dispose();
            _renderer = null;
        }

        /**
         * Fired when the emulator has a full frame to be rendered.
         * This should occur at approximately 60hz.
         */
        private void SpaceInvaders_OnRender(RenderEventArgs eventArgs)
        {
            _renderer.QueueFrame(eventArgs.FrameBuffer);
        }

        /**
         * Fired when the emulator needs to play a sound.
         */
        private void SpaceInvaders_OnSound(SoundEventArgs eventArgs)
        {
            // TODO: Implement sound output.
            // Resolver.Log.Info("SpaceInvaders_OnSound fired!");
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
                Resolver.Log.Info($"[STATS] Overbudget: Average time to execute to vsync was {averageMs} (> 16.6 ms)");
            }
            else
            {
                Resolver.Log.Info($"[STATS] Underbudget: Average time to execute to vsync was {averageMs} (< 16.6 ms)");
            }

            if (_configuration.HaltAfterRecordedStatCount >= 0 && _statCount >= _configuration.HaltAfterRecordedStatCount)
            {
                Resolver.Log.Info($"[STATS] Stopping emulator after {_configuration.HaltAfterRecordedStatCount} statistic reports");
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
