
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace JustinCredible.SIEmulator.Tests
{
    public class BaseTest
    {
        protected static byte[] AssembleSource(string source)
        {
            var tempFilePath = Path.GetTempFileName();
            var sourceFilePath = tempFilePath;
            var romFilePath = Path.Combine(Path.GetDirectoryName(tempFilePath), Path.GetFileNameWithoutExtension(tempFilePath) + ".rom");

            File.WriteAllText(sourceFilePath, source);

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "/Users/justin/Downloads/zasm-4.2.4-macos10.12/zasm"; // TODO: Read from env var or something.
            startInfo.Arguments = $"--asm8080 \"{sourceFilePath}\"";
            startInfo.RedirectStandardError = true;

            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            var stdErr = process.StandardError.ReadToEnd();
            var exitCode = process.ExitCode;

            if (exitCode != 0)
                throw new Exception($"Error assembling Intel 8080 source code; non-zero exit code: {exitCode}; stdErr: {stdErr}");

            return File.ReadAllBytes(romFilePath);
        }

        protected UnitTestCPUState Execute(byte[] rom, CPUState cpuState = null)
        {
            var emulator = new CPU();
            emulator.Reset();
            emulator.LoadRom(rom);

            if (cpuState != null)
            {
                emulator.LoadRegisters(cpuState.Registers);
            }

            var iterations = 0;
            var cycles = 0;
            var pcAddresses = new List<UInt16>();

            while (!emulator.Finished)
            {
                if (iterations > 100)
                    throw new Exception("More than 100 iterations occurred.");

                pcAddresses.Add(emulator.DumpState().ProgramCounter);

                cycles += emulator.Step();

                iterations++;
            }

            var state = emulator.DumpState();

            var extendedState = new UnitTestCPUState(state)
            {
                Iterations = iterations,
                Cycles = cycles,
                ProgramCounterAddresses = pcAddresses,
            };

            return extendedState;
        }
    }
}
