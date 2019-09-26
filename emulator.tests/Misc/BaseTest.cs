
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xunit;

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

        protected CPUStats Execute(byte[] rom, InitialCPUState initialState = null)
        {
            var cpu = new CPU();
            cpu.Reset();

            // Ensure the ROM data is not larger than we can load.
            if (rom.Length > 8192)
                throw new Exception("ROM filesize cannot exceed 8 kilobytes.");

            // Build the memory map manually, so that if a unit tests provides the RAM we can
            // just map it in below.
            var memory = new byte[16384];

            // The ROM is the lower 8K of addressable memory.
            Array.Copy(rom, memory, rom.Length);

            // Map in the registers and/or RAM if the unit test provided them.
            if (initialState != null)
            {
                if (initialState.Memory != null)
                {
                    if (rom.Length > 16384)
                        throw new Exception("Memory cannot exceed 16 kilobytes.");

                    // Copy the RAM portion (upper 8K) into the memory map that already contains
                    // the ROM in the lower 8K.
                    Array.Copy(initialState.Memory, 8192, memory, 8192, 8192);
                }

                if (initialState.Registers != null)
                    cpu.Registers = initialState.Registers;

                if (initialState.Flags != null)
                    cpu.Flags = initialState.Flags;

                if (initialState.StackPointer != null)
                    cpu.StackPointer = initialState.StackPointer.Value;

                if (initialState.ProgramCounter != null)
                    cpu.ProgramCounter = initialState.ProgramCounter.Value;
            }

            // Finally, set the built memory map into the CPU.
            cpu.LoadMemory(memory);

            // Record the number of iterations (instructions), CPU cycles, and the address of teh
            // program counter after each instruction is executed. This allows tests to assert each
            // of these values in addition to the CPU state.
            var iterations = 0;
            var cycles = 0;
            var pcAddresses = new List<UInt16>();

            while (!cpu.Finished)
            {
                // Ensure we don't have a run away program.
                if (iterations > 100)
                    throw new Exception("More than 100 iterations occurred.");

                pcAddresses.Add(cpu.ProgramCounter);

                cycles += cpu.Step();

                iterations++;
            }

            // Return the state of the CPU so tests can do verification.

            var results = new CPUStats()
            {
                Iterations = iterations,
                Cycles = cycles,
                ProgramCounterAddresses = pcAddresses,
                Memory = cpu.Memory,
                Registers = cpu.Registers,
                Flags = cpu.Flags,
                ProgramCounter = cpu.ProgramCounter,
                StackPointer = cpu.StackPointer,
            };

            return results;
        }

        protected void AssertFlagsFalse(CPUStats stats)
        {
            Assert.False(stats.Flags.AuxCarry);
            Assert.False(stats.Flags.Carry);
            Assert.False(stats.Flags.Parity);
            Assert.False(stats.Flags.Sign);
            Assert.False(stats.Flags.Zero);
        }
    }
}
