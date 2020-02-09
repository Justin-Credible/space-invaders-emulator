using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class XTHLTests : BaseTest
    {
        [Fact]
        public void TestXTHL_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                XTHL
                HLT
            ");

            var memory = new byte[16384];
            memory[0x2222] = 0x99;
            memory[0x2223] = 0x88;

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    H = 0x42,
                    L = 0x77,
                },
                StackPointer = 0x2222,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x88, state.Registers.H);
            Assert.Equal(0x99, state.Registers.L);
            Assert.Equal(0x77, state.Memory[0x2222]);
            Assert.Equal(0x42, state.Memory[0x2223]);
            Assert.Equal(0x2222, state.StackPointer);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 18, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
