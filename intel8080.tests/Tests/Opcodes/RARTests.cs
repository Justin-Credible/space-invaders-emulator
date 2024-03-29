using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class RARTests : BaseTest
    {
        [Theory]
        [InlineData(0b01001001, false, 0b00100100, true)]
        [InlineData(0b01001001, true, 0b10100100, true)]
        [InlineData(0b01001000, false, 0b00100100, false)]
        [InlineData(0b01001000, true, 0b10100100, false)]
        public void TestRAR(byte initialValue, bool initialCarryFlag, byte expectedValue, bool expectedCarryFlag)
        {
            var rom = AssembleSource($@"
                org 00h
                RAR
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
