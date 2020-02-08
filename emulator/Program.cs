using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace JustinCredible.SIEmulator
{
    class Program
    {
        private static CommandLineApplication _app;

        private static SpaceInvaders _game;

        private static bool _guiClosed = false;

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

            command.OnExecute(() =>
            {
                if (String.IsNullOrWhiteSpace(romPathArg.Value))
                    throw new Exception("A directory containing invaders.e though .h files is required.");

                if (!Directory.Exists(romPathArg.Value))
                    throw new Exception($"Could not locate a directory at path {romPathArg.Value}");

                var rom = ReadRomFiles(romPathArg.Value);

                var gui = new GUI();
                gui.Initialize("Space Invaders Emulator", SpaceInvaders.RESOLUTION_WIDTH, SpaceInvaders.RESOLUTION_HEIGHT, 10, 10);
                gui.OnTick += GUI_OnTick;

                _game = new SpaceInvaders();
                _game.OnRender += SpaceInvaders_OnRender;
                _game.Start(rom);

                gui.StartLoop();
                gui.Dispose();
                _guiClosed = true;

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

        private static void SpaceInvaders_OnRender(RenderEventArgs eventArgs)
        {
            _frameBuffer = eventArgs.FrameBuffer;
            _renderFrameNextTick = true;
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
