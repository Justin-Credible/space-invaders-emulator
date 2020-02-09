﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Extensions.CommandLineUtils;

namespace JustinCredible.SIEmulator
{
    class Program
    {
        private static CommandLineApplication _app;

        private static SpaceInvaders _game;

        // Used to pass data from the emulator thread's loop to the GUI loop.
        private static byte[] _frameBuffer;
        private static bool _renderFrameNextTick = false;

        // Used to pass data from the GUI event loop to the emulator thread's loop.
        private static Dictionary<byte, bool> _keys = null;

        public static void Main(string[] args)
        {
            var version = Utilities.AppVersion;

            _app = new CommandLineApplication();
            _app.Name = "siemu";
            _app.Description = "Space Invaders Emulator";
            _app.HelpOption("-?|-h|--help");

            _app.VersionOption("-v|--version",

                // Used for HelpOption() header
                $"{_app.Name} {version}",

                // Used for output of --version option.
                version
            );

            // When launched without any commands or options.
            _app.OnExecute(() =>
            {
                _app.ShowHelp();
                return 0;
            });

            _app.Command("run", Run);

            _app.Execute(args);
        }

        private static void Run(CommandLineApplication command)
        {
            command.Description = "Runs the emulator using the given ROM file.";
            command.HelpOption("-?|-h|--help");

            var romPathArg = command.Argument("[ROM path]", "The path to a directory containing the Space Invaders ROM set to load.");

            var debugOption = command.Option("-d|--debug", "Run in debug mode; enables internal statistics and logs useful when debugging.", CommandOptionType.NoValue);

            command.OnExecute(() =>
            {
                if (String.IsNullOrWhiteSpace(romPathArg.Value))
                    throw new Exception("A directory containing invaders.e though .h files is required.");

                if (!Directory.Exists(romPathArg.Value))
                    throw new Exception($"Could not locate a directory at path {romPathArg.Value}");

                var rom = ReadRomFiles(romPathArg.Value);

                Thread.CurrentThread.Name = "GUI Loop";

                // Initialize the user interface (window) and wire an event handler
                // that will handle receiving user input as well as sending the
                // framebuffer to be rendered. Note that we pass the weidth/height
                // parameters backwards on purpose because the CRT screen in the
                // cabinet is rotated to the left (-90 degrees) for a 3:4 display.
                var gui = new GUI();
                gui.Initialize("Space Invaders Emulator", SpaceInvaders.RESOLUTION_HEIGHT, SpaceInvaders.RESOLUTION_WIDTH, 2, 2);
                gui.OnTick += GUI_OnTick;

                // Initialize the Space Invaders hardware/emulator and wire an event
                // handler so receive the framebuffer to be rendered.
                _game = new SpaceInvaders();
                _game.OnRender += SpaceInvaders_OnRender;

                if (debugOption.HasValue())
                    _game.Debug = true;

                // Start the emulation; this occurs in a seperate thread and
                // therefore this call is non-blocking.
                _game.Start(rom);

                // Starts the event loop for the user interface; this occurs on
                // the same thread and is a blocking call. Once this method returns
                // we know that the user closed the window or quit the program via
                // the OS (e.g. ALT+F4 / CMD+Q).
                gui.StartLoop();

                // Ensure the GUI resources are cleaned up and stop the emulation.
                gui.Dispose();
                _game.Stop();

                return 0;
            });
        }

        private static byte[] ReadRomFiles(string directoryPath)
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

        /**
         * Fired when the emulator has a full frame to be rendered.
         * This should occur at approximately 60hz.
         */
        private static void SpaceInvaders_OnRender(RenderEventArgs eventArgs)
        {
            _frameBuffer = eventArgs.FrameBuffer;
            _renderFrameNextTick = true;
        }

        /**
         * Fired when the GUI event loop "ticks". This provides an opportunity
         * to receive user input as well as send the framebuffer to be rendered.
         */
        private static void GUI_OnTick(GUITickEventArgs eventArgs)
        {
            _keys = eventArgs.Keys;

            if (_renderFrameNextTick)
            {
                eventArgs.FrameBuffer = _frameBuffer;
                eventArgs.ShouldRender = true;
                _renderFrameNextTick = false;
            }
        }
    }
}
