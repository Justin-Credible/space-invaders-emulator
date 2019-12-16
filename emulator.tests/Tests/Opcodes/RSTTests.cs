using System;
using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class RSTTests : BaseTest
    {
        [Theory]
        [InlineData(0, 0x0000)]
        [InlineData(1, 0x0008)]
        [InlineData(2, 0x0010)]
        [InlineData(3, 0x0018)]
        [InlineData(4, 0x0020)]
        [InlineData(5, 0x0028)]
        [InlineData(6, 0x0030)]
        [InlineData(7, 0x0038)]
        public void TestRST(int resetIndex, UInt16 expectedProgramCounterValue)
        {
            var rom = AssembleSource($@"
                org 00h
                HLT

                org 08h
                HLT

                org 010h
                HLT

                org 018h
                HLT

                org 20h
                HLT

                org 28h
                HLT

                org 30h
                HLT

                org 38h
                HLT

                org 40h
                RST {resetIndex}
            ");

            var memory = new byte[16384];
            memory[0x2720] = 0xFF;
            memory[0x271F] = 0xFF;
            memory[0x271E] = 0xFF;
            memory[0x271D] = 0xFF;

            var initialState = new CPUConfig()
            {
                ProgramCounter = 0x0040,
                StackPointer = 0x2720,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x271E, state.StackPointer);
            Assert.Equal(0xFF, state.Memory[0x2720]);
            Assert.Equal(0x00, state.Memory[0x271F]);
            Assert.Equal(0x40, state.Memory[0x271E]);
            Assert.Equal(0xFF, state.Memory[0x271D]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 11, state.Cycles);
            Assert.Equal(expectedProgramCounterValue, state.ProgramCounter);
        }

        [Fact]
        public void TestRSTAndRET()
        {
            var rom = AssembleSource($@"
                org 00h     ; RST 0
                MVI A, 1D
                RET

                org 08h     ; RST 1
                MVI B, 2D
                RET

                org 010h    ; RST 2
                MVI C, 3D
                RET

                org 018h    ; RST 3
                MVI D, 4D
                RET

                org 20h     ; RST 4
                MVI E, 5D
                RET

                org 28h     ; RST 5
                MVI H, 6D
                RET

                org 30h     ; RST 6
                RST 7
                RET

                org 38h     ; RST 7
                RST 5
                RET

                MVI L, 7D

                org 40h
                RST 0       ; 0x40
                RST 1       ; 0x41
                RST 2       ; 0x42
                RST 3       ; 0x43
                RST 4       ; 0x44
                RST 6       ; 0x45
                HLT         ; 0x46
            ");

            var memory = new byte[16384];
            memory[0x2720] = 0xFF;
            memory[0x271F] = 0xFF;
            memory[0x271E] = 0xFF;
            memory[0x271D] = 0xFF;
            memory[0x271C] = 0xFF;
            memory[0x271B] = 0xFF;
            memory[0x271A] = 0xFF;
            memory[0x2719] = 0xFF;

            var initialState = new CPUConfig()
            {
                ProgramCounter = 0x0040,
                StackPointer = 0x2720,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x2720, state.StackPointer);
            Assert.Equal(0xFF, state.Memory[0x2720]);
            Assert.Equal(0x00, state.Memory[0x271F]);
            Assert.Equal(0x45, state.Memory[0x271E]);
            Assert.Equal(0x00, state.Memory[0x271D]);
            Assert.Equal(0x30, state.Memory[0x271C]);
            Assert.Equal(0x00, state.Memory[0x271B]);
            Assert.Equal(0x38, state.Memory[0x271A]);
            Assert.Equal(0xFF, state.Memory[0x2719]);

            AssertFlagsFalse(state);

            Assert.Equal(23, state.Iterations);
            Assert.Equal(7 + (8*11) + (8*10) + (6*7), state.Cycles);
            Assert.Equal(0x46, state.ProgramCounter);
        }
    }
}
