using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class STATests : BaseTest
    {
        [Fact]
        public void TestSTA()
        {
            var rom = AssembleSource($@"
                org 00h
                STA 2477h
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x42;

            var initialState = new CPUConfig()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x42, state.Memory[0x2477]);
            Assert.Equal(0x42, state.Registers.A);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 13, state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }
    }
}
