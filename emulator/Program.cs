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

        // Flags set via command line arguments.
        private static int _speed = 10;
        private static bool _debug = false;
        private static bool _logPerformance = false;
        private static bool _keepOpenWhenDoneEmulating = false;

        private static CPU _cpu;
        private static bool _shouldBreak = false;
        private static bool _shouldStep = false;

        private static bool _guiClosed = false;
        private static Stopwatch _guiPerformanceWatch;
        private static int _guiTickCounter;

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

            var romPathArg = command.Argument("[ROM path]", "The path to a ROM file to load.");

            var speedOption = command.Option("-s|--speed", "Controls how fast the emulator should run (100 = max, 1 = min, 50 = default)", CommandOptionType.SingleValue);
            var debugOption = command.Option("-d|--debug", "Run in debug mode; Space Bar = Break, F10 = Step, F5 = Continue", CommandOptionType.NoValue);
            var breakOption = command.Option("-b|--break", "Breakpoint on startup; requires debug option to function.", CommandOptionType.NoValue);
            var performanceOption = command.Option("-p|--perfmon", "Performance monitor; write stats to the console while running.", CommandOptionType.NoValue);
            var keepOpenOption = command.Option("-ko|--keep-open", "Keep the GUI open even if the emulator finishes ROM execution.", CommandOptionType.NoValue);

            command.OnExecute(() =>
            {
                byte[] rom;

                // TODO: Read directory of ROM files.
                if (File.Exists(romPathArg.Value))
                    rom = System.IO.File.ReadAllBytes(romPathArg.Value);
                else
                    throw new Exception($"Could not locate a ROM file at path {romPathArg.Value}");

                _speed = 50;

                if (speedOption.HasValue())
                {
                    bool parsed = int.TryParse(speedOption.Value(), out _speed);

                    if (!parsed || _speed < 1 || _speed > 100)
                        throw new Exception("Speed must be between 1 and 100, inclusive.");
                }

                if (debugOption.HasValue())
                {
                    _debug = true;

                    if (breakOption.HasValue())
                        _shouldBreak = true;
                }

                if (performanceOption.HasValue())
                    _logPerformance = true;

                if (keepOpenOption.HasValue())
                    _keepOpenWhenDoneEmulating = true;

                // TODO: Get ROM file path via standard File > Open dialog if one not specified
                // via the command line arguments.
                _cpu = new CPU();
                _cpu.LoadRom(rom);

                var gui = new GUI();
                gui.Initialize("Space Invaders Emulator", SpaceInvaders.RESOLUTION_WIDTH, SpaceInvaders.RESOLUTION_HEIGHT, 10, 10);
                gui.OnTick += GUI_OnTick;

                if (_logPerformance)
                {
                    _guiTickCounter = 0;
                    _guiPerformanceWatch = new Stopwatch();
                    _guiPerformanceWatch.Start();
                }

                var emulatorThread = new Thread(new ThreadStart(EmulatorLoop));
                emulatorThread.Start();

                gui.StartLoop();
                gui.Dispose();
                _guiClosed = true;

                return 0;
            });
        }

        private static void EmulatorLoop()
        {
            var stepCounter = 0;
            Stopwatch performanceWatch = null;

            if (_logPerformance)
            {
                performanceWatch = new Stopwatch();
                performanceWatch.Start();
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Setup the delay used between each emulator opcode step.
            var stepDelay = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / (_speed * 10));

            while (!_cpu.Finished && !_guiClosed)
            {
                var elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

                // If we're running in debug mode, dump the PC, registers, etc and
                // then wait until the step key is pressed before continuing.
                if (_debug && _shouldBreak)
                {
                    _cpu.PrintDebugSummary();

                    while (_shouldBreak)
                    {
                        if (_guiClosed)
                            return;

                        Thread.Sleep(250);
                    }

                    if (_shouldStep)
                        _shouldBreak = true;
                }

                stopwatch.Restart();

                // _cpu.Step(elapsedMilliseconds, _keys);
                _cpu.Step();

                // if (_cpu.FrameBufferUpdated)
                // {
                //     _frameBuffer = _cpu.FrameBuffer;
                //     _renderFrameNextTick = true;
                // }

                if (_logPerformance)
                {
                    stepCounter++;

                    if (performanceWatch.ElapsedMilliseconds >= 1000)
                    {
                        Console.WriteLine("Opcodes per second: " + stepCounter);
                        stepCounter = 0;
                        performanceWatch.Restart();
                    }
                }

                Thread.Sleep(stepDelay);
            }
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

            if (_cpu.Finished && !_keepOpenWhenDoneEmulating)
                eventArgs.ShouldQuit = true;

            if (_logPerformance)
            {
                _guiTickCounter++;

                if (_guiPerformanceWatch.ElapsedMilliseconds >= 1000)
                {
                    Console.WriteLine("GUI ticks per second: " + _guiTickCounter);
                    _guiTickCounter = 0;
                    _guiPerformanceWatch.Restart();
                }
            }

            if (_debug && eventArgs.KeyDown != null)
            {
                if (eventArgs.KeyDown == SDL2.SDL.SDL_Keycode.SDLK_SPACE)
                {
                    _shouldBreak = true;
                }
                else if (eventArgs.KeyDown == SDL2.SDL.SDL_Keycode.SDLK_F5)
                {
                    _shouldStep = false;
                    _shouldBreak = false;
                }
                else if (eventArgs.KeyDown == SDL2.SDL.SDL_Keycode.SDLK_F10)
                {
                    _shouldStep = true;
                    _shouldBreak = false;
                }
            }
        }
    }
}
