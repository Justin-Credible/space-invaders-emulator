using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class EmulationStressTests
    {
        [Fact]
        public void RapidInputSpamDoesNotCrashEmulation()
        {
            var rom = ReadInvadersRomFromWorkspace();
            var emulator = new SpaceInvaders();
            var stopped = new ManualResetEventSlim(false);

            emulator.OnEmulationStopped += () => stopped.Set();

            emulator.Start(rom);

            try
            {
                var watch = Stopwatch.StartNew();
                var iteration = 0;

                // Toggle many combinations quickly, with frequent coin + P1 start pulses.
                while (watch.Elapsed < TimeSpan.FromSeconds(2))
                {
                    var pattern = iteration % 32;

                    emulator.ButtonCredit = (pattern % 3) == 0;
                    emulator.ButtonStart1P = (pattern % 5) <= 1;
                    emulator.ButtonStart2P = (pattern % 11) == 0;
                    emulator.ButtonP1Left = (pattern % 2) == 0;
                    emulator.ButtonP1Right = (pattern % 4) <= 1;
                    emulator.ButtonP1Fire = (pattern % 3) == 1;
                    emulator.ButtonP2Left = (pattern % 7) <= 2;
                    emulator.ButtonP2Right = (pattern % 9) <= 3;
                    emulator.ButtonP2Fire = (pattern % 5) == 2;
                    emulator.ButtonTilt = (pattern % 23) == 0;

                    if (iteration % 17 == 0)
                    {
                        emulator.ButtonCredit = true;
                        emulator.ButtonStart1P = true;
                    }

                    Thread.Sleep(1);
                    iteration++;
                }
            }
            finally
            {
                emulator.ButtonCredit = false;
                emulator.ButtonStart1P = false;
                emulator.ButtonStart2P = false;
                emulator.ButtonP1Left = false;
                emulator.ButtonP1Right = false;
                emulator.ButtonP1Fire = false;
                emulator.ButtonP2Left = false;
                emulator.ButtonP2Right = false;
                emulator.ButtonP2Fire = false;
                emulator.ButtonTilt = false;

                emulator.Stop();

                Assert.True(stopped.Wait(TimeSpan.FromSeconds(5)), "Emulator did not stop cleanly.");
            }
        }

        private static byte[] ReadInvadersRomFromWorkspace()
        {
            var root = FindWorkspaceRoot(AppContext.BaseDirectory);
            var romDirectory = Path.Combine(root, "roms");

            var hPath = Path.Combine(romDirectory, "invaders.h");
            var gPath = Path.Combine(romDirectory, "invaders.g");
            var fPath = Path.Combine(romDirectory, "invaders.f");
            var ePath = Path.Combine(romDirectory, "invaders.e");

            if (!File.Exists(hPath) || !File.Exists(gPath) || !File.Exists(fPath) || !File.Exists(ePath))
                throw new Exception($"Could not locate all ROM files in '{romDirectory}'.");

            var bytes = new List<byte>();
            bytes.AddRange(File.ReadAllBytes(hPath));
            bytes.AddRange(File.ReadAllBytes(gPath));
            bytes.AddRange(File.ReadAllBytes(fPath));
            bytes.AddRange(File.ReadAllBytes(ePath));

            return bytes.ToArray();
        }

        private static string FindWorkspaceRoot(string startPath)
        {
            var directory = new DirectoryInfo(startPath);

            while (directory != null)
            {
                var hasSolutionFolders =
                    Directory.Exists(Path.Combine(directory.FullName, "emulator")) &&
                    Directory.Exists(Path.Combine(directory.FullName, "roms"));

                if (hasSolutionFolders)
                    return directory.FullName;

                directory = directory.Parent;
            }

            throw new Exception("Unable to locate workspace root that contains emulator and roms directories.");
        }
    }
}
