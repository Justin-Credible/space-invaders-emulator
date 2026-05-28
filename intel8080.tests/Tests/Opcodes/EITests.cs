using Xunit;
using System;

namespace JustinCredible.Intel8080.Tests
{
    public class EITests : BaseTest
    {
        [Fact]
        public void TestEI()
        {
            var rom = AssembleSource($@"
                org 00h
                EI
                HLT
            ");

            var initialState = new CPUConfig()
            {
                InterruptsEnabled = false,
            };

            var state = Execute(rom, initialState);

            Assert.True(state.InterruptsEnabled);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestEIEnableDelayedUntilAfterNextInstruction()
        {
            var rom = AssembleSource($@"
                org 00h
                EI
                NOP
                HLT
            ");

            var config = new CPUConfig()
            {
                InterruptsEnabled = false,
            };

            var cpu = new CPU(config);
            var memory = new byte[config.MemorySize];
            Array.Copy(rom, memory, rom.Length);
            cpu.LoadMemory(memory);

            cpu.Step(); // EI
            Assert.False(cpu.InterruptsEnabled);

            cpu.Step(); // NOP
            Assert.True(cpu.InterruptsEnabled);

            cpu.Step(); // HLT
            Assert.True(cpu.InterruptsEnabled);
            Assert.True(cpu.Finished);
        }

        [Fact]
        public void TestDICancelsPendingEIEnable()
        {
            var rom = AssembleSource($@"
                org 00h
                EI
                DI
                NOP
                HLT
            ");

            var config = new CPUConfig()
            {
                InterruptsEnabled = false,
            };

            var cpu = new CPU(config);
            var memory = new byte[config.MemorySize];
            Array.Copy(rom, memory, rom.Length);
            cpu.LoadMemory(memory);

            cpu.Step(); // EI
            Assert.False(cpu.InterruptsEnabled);

            cpu.Step(); // DI
            Assert.False(cpu.InterruptsEnabled);

            cpu.Step(); // NOP
            Assert.False(cpu.InterruptsEnabled);

            cpu.Step(); // HLT
            Assert.True(cpu.Finished);
            Assert.False(cpu.InterruptsEnabled);
        }
    }
}
