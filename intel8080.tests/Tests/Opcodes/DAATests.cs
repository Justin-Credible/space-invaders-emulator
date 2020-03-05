using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class DAA : BaseTest
    {
        // This covers the example from the 8080 Programmers Manual.
        [Fact]
        public void TestDAA_DoubleCarry()
        {
            var rom = AssembleSource($@"
                org 00h
                DAA
                HLT
            ");

            var registers = new CPURegisters()
            {
                A = 0x9B,
            };

            var flags = new ConditionFlags()
            {
                Zero = true,
                Sign = true,
                Parity = true,
                Carry = false,
                AuxCarry = false,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            // Ensure these flags were updated.
            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);

            // Both carry bits are set in this case.
            Assert.True(state.Flags.Carry);
            Assert.True(state.Flags.AuxCarry);

            // Verify accumulator value.
            Assert.Equal(1, state.Registers.A);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
