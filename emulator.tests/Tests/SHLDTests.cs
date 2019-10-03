using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class SHLDTests : BaseTest
    {
        [Fact]
        public void TestSHLD()
        {
            var rom = AssembleSource($@"
                org 00h
                SHLD 2477h
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    H = 0x66,
                    L = 0x77,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x77, state.Memory[0x2477]);
            Assert.Equal(0x66, state.Memory[0x2478]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 16, state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }
    }
}
