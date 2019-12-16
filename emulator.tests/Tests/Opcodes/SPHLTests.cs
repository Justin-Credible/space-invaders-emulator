using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class SPHLTests : BaseTest
    {
        [Fact]
        public void TestSPHL()
        {
            var rom = AssembleSource($@"
                org 00h
                SPHL
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    HL = 0x2477,
                },
            };

            var state = Execute(rom, initialState);

            AssertFlagsFalse(state);

            Assert.Equal(0x2477, state.StackPointer);
            Assert.Equal(0x2477, state.Registers.HL);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 5, state.Cycles);
            Assert.Equal(0x0001, state.ProgramCounter);
        }
    }
}
