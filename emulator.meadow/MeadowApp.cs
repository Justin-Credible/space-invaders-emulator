using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;

namespace JustinCredible.SIEmulator.MeadowMCU
{
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        private RgbPwmLed _onboardLed;
        private SpaceInvaders _game;

        public MeadowApp()
        {
            try
            {
                Initialize();
                Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unhandled exception occurred!");
                _onboardLed.StartPulse(Color.Red);

                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception:");
                    Console.WriteLine(ex.InnerException.Message);
                    Console.WriteLine(ex.InnerException.StackTrace);
                }
            }
        }

        private void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            _onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);
        }

        private void Run()
        {
            Console.WriteLine("Starting MCU application code...");
            _onboardLed.StartPulse(Color.Yellow);

            Console.WriteLine("Reading ROM files...");
            var rom = ReadRomFiles(MeadowOS.FileSystem.DataDirectory);

            Console.WriteLine("Initializing emulator...");
            _game = new SpaceInvaders();
            _game.OnEmulationStopped += SpaceInvaders_OnEmulationStopped;
            _game.OnRender += SpaceInvaders_OnRender;
            _game.OnSound += SpaceInvaders_OnSound;
            _game.StatsEnabled = true;

            // Start the emulation; this occurs in a seperate thread and
            // therefore this call is non-blocking.
            Console.WriteLine("Running emulator...");
            _game.Start(rom);

            Console.WriteLine("Emulator is running!");
            Thread.Sleep(Timeout.Infinite);
        }

        #region Emulator Event Handlers

        private void SpaceInvaders_OnEmulationStopped()
        {
            Console.WriteLine("Emulator stopped!");
            _onboardLed.StartPulse(Color.Purple);
        }

        /**
         * Fired when the emulator has a full frame to be rendered.
         * This should occur at approximately 60hz.
         */
        private void SpaceInvaders_OnRender(RenderEventArgs eventArgs)
        {
            Console.WriteLine("SpaceInvaders_OnRender fired!");
        }

        /**
         * Fired when the emulator needs to play a sound.
         */
        private void SpaceInvaders_OnSound(SoundEventArgs eventArgs)
        {
            Console.WriteLine("SpaceInvaders_OnSound fired!");
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
