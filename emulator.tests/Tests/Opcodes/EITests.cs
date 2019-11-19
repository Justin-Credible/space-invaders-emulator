using Xunit;

namespace JustinCredible.SIEmulator.Tests
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

            var initialState = new InitialCPUState()
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
    }
}