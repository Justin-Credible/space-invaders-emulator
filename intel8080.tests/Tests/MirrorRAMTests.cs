using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class MirrorRAMTests : BaseTest
    {
        [Fact]
        public void TestReadMirrorRAMAddresses()
        {
            var rom = AssembleSource($@"
                org 00h
                LXI H, 4000h
                MOV A, M
                LXI H, 4001h
                MOV B, M
                LXI H, 5FFDh
                MOV C, M
                LXI H, 5FFEh
                MOV D, M
                LXI H, 5FFFh
                MOV E, M
                HLT
            ");

            var memory = new byte[16384];
            memory[0x2000] = 0x22;
            memory[0x2001] = 0x33;
            memory[0x3FFD] = 0x44;
            memory[0x3FFE] = 0x55;
            memory[0x3FFF] = 0x66;

            var initialState = new CPUConfig()
            {
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x22, state.Registers.A);
            Assert.Equal(0x33, state.Registers.B);
            Assert.Equal(0x44, state.Registers.C);
            Assert.Equal(0x55, state.Registers.D);
            Assert.Equal(0x66, state.Registers.E);

            AssertFlagsFalse(state);

            Assert.Equal(11, state.Iterations);
            Assert.Equal((7*5) + (10*5) + 7, state.Cycles);
            Assert.Equal(0x14, state.ProgramCounter);
        }

        [Fact]
        public void TestWriteMirrorRAMAddresses()
        {
            var rom = AssembleSource($@"
                org 00h
                LXI H, 4000h
                MOV M, A
                LXI H, 4001h
                MOV M, B
                LXI H, 5FFDh
                MOV M, C
                LXI H, 5FFEh
                MOV M, D
                LXI H, 5FFFh
                MOV M, E
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x22,
                    B = 0x33,
                    C = 0x44,
                    D = 0x55,
                    E = 0x66,
                }
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x22, state.Memory[0x2000]);
            Assert.Equal(0x33, state.Memory[0x2001]);
            Assert.Equal(0x44, state.Memory[0x3FFD]);
            Assert.Equal(0x55, state.Memory[0x3FFE]);
            Assert.Equal(0x66, state.Memory[0x3FFF]);

            AssertFlagsFalse(state);

            Assert.Equal(11, state.Iterations);
            Assert.Equal((7*5) + (10*5) + 7, state.Cycles);
            Assert.Equal(0x14, state.ProgramCounter);
        }
    }
}
