using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Extensions.CommandLineUtils;

namespace JustinCredible.SIEmulator
{
    class Program
    {
        private static CommandLineApplication _app;

        private static SpaceInvaders _game;

        private static bool _guiClosed = false;

        // Used to pass data from the emulator thread's loop to the GUI loop.
        private static byte[,] _frameBuffer;
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

            command.OnExecute(() =>
            {
                byte[] rom;

                // TODO: Read directory of ROM files.
                if (File.Exists(romPathArg.Value))
                    rom = System.IO.File.ReadAllBytes(romPathArg.Value);
                else
                    throw new Exception($"Could not locate a ROM file at path {romPathArg.Value}");

                var gui = new GUI();
                gui.Initialize("Space Invaders Emulator", SpaceInvaders.RESOLUTION_WIDTH, SpaceInvaders.RESOLUTION_HEIGHT, 10, 10);
                gui.OnTick += GUI_OnTick;

                // TODO: Glue Program; initialize SpaceInvaders class here instead of the CPU core.
                // TODO: Get ROM file path via standard File > Open dialog if one not specified
                // via the command line arguments.
                _game = new SpaceInvaders();
                _game.Start(rom);

                gui.StartLoop();
                gui.Dispose();
                _guiClosed = true;

                return 0;
            });
        }

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
