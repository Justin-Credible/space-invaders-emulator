using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class XCHGTests : BaseTest
    {
        [Fact]
        public void TestXCHG_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                XCHG
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    H = 0x42,
                    D = 0x99,
                    L = 0x77,
                    E = 0x88,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x99, state.Registers.H);
            Assert.Equal(0x42, state.Registers.D);
            Assert.Equal(0x88, state.Registers.L);
            Assert.Equal(0x77, state.Registers.E);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 5, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
