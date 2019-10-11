using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class RLCTests : BaseTest
    {
        [Fact]
        public void TestRLC_SetsCarryFlagTrue()
        {
            var rom = AssembleSource($@"
                org 00h
                RLC
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b11100100,
                },
                Flags = new ConditionFlags()
                {
                    Carry = false,
                }
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b11001001, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestRRC_SetsCarryFlagFalse()
        {
            var rom = AssembleSource($@"
                org 00h
                RLC
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b01100101,
                },
                Flags = new ConditionFlags()
                {
                    Carry = true,
                }
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b11001010, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
