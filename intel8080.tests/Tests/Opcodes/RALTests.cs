using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class RALTests : BaseTest
    {
        [Theory]
        [InlineData(0b11001001, false, 0b10010010, true)]
        [InlineData(0b11001001, true, 0b10010011, true)]
        [InlineData(0b01001000, false, 0b10010000, false)]
        [InlineData(0b01001000, true, 0b10010001, false)]
        public void TestRAL(byte initialValue, bool initialCarryFlag, byte expectedValue, bool expectedCarryFlag)
        {
            var rom = AssembleSource($@"
                org 00h
                RAL
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = initialValue,
                },
                Flags = new ConditionFlags()
                {
                    Carry = initialCarryFlag,
                }
            };

            var state = Execute(rom, initialState);

            Assert.Equal(expectedValue, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.Equal(expectedCarryFlag, state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
