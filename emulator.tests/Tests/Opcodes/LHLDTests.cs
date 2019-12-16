using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class LHLDTests : BaseTest
    {
        [Fact]
        public void TestLHLD()
        {
            var rom = AssembleSource($@"
                org 00h
                LHLD 2477h
                HLT
            ");

            var memory = new byte[16384];
            memory[0x2477] = 0x77;
            memory[0x2478] = 0x66;

            var initialState = new CPUConfig()
            {
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x66, state.Registers.H);
            Assert.Equal(0x77, state.Registers.L);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 16, state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }
    }
}
