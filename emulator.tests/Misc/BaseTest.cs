
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
            startInfo.FileName = "../../../../assembler/zasm-4.2.4-macos10.12/zasm";
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

            // Ensure the ROM data is not larger than we can load.
            if (rom.Length > 8192)
                throw new Exception("ROM filesize cannot exceed 8 kilobytes.");

            // Build the memory map manually, so that if a unit tests provides the RAM we can
            // just map it in below.
            var memory = new byte[16384];

            // The ROM is the lower 8K of addressable memory.
            Array.Copy(rom, memory, rom.Length);

            // Map in the registers and/or RAM if the unit test provided them.
            if (cpuState != null)
            {
                emulator.LoadRegisters(cpuState.Registers);

                if (cpuState.Memory != null)
                {
                    if (rom.Length > 16384)
                        throw new Exception("Memory cannot exceed 16 kilobytes.");

                    // Copy the RAM portion (upper 8K) into the memory map that already contains
                    // the ROM in the lower 8K.
                    Array.Copy(cpuState.Memory, 8192, memory, 8192, 8192);
                }
            }

            // Finally, set the built memory map into the CPU.
            emulator.LoadMemory(memory);

            // Record the number of iterations (instructions), CPU cycles, and the address of teh
            // program counter after each instruction is executed. This allows tests to assert each
            // of these values in addition to the CPU state.
            var iterations = 0;
            var cycles = 0;
            var pcAddresses = new List<UInt16>();

            while (!emulator.Finished)
            {
                // Ensure we don't have a run away program.
                if (iterations > 100)
                    throw new Exception("More than 100 iterations occurred.");

                pcAddresses.Add(emulator.DumpState().ProgramCounter);

                cycles += emulator.Step();

                iterations++;
            }

            // Return the state of the CPU so tests can do verification.

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
