﻿using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;

namespace JustinCredible.SIEmulator.MeadowMCU
{
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        RgbPwmLed onboardLed;

        public MeadowApp()
        {
            Initialize();
            Run();
        }

        private void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);
        }

        private void Run()
        {
            Console.WriteLine("Running...");

            onboardLed.StartPulse(Color.Green);

            Console.WriteLine("List data directory files: " + MeadowOS.FileSystem.DataDirectory);

            var files = System.IO.Directory.GetFiles(MeadowOS.FileSystem.DataDirectory);
            foreach (var file in files)
            {
                Console.WriteLine(" • " + file);
            }
        }
    }
}
