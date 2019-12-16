using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class RRCTests : BaseTest
    {
        [Fact]
        public void TestRRC_SetsCarryFlagTrue()
        {
            var rom = AssembleSource($@"
                org 00h
                RRC
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0b01100101,
                },
                Flags = new ConditionFlags()
                {
                    Carry = false,
                }
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b10110010, state.Registers.A);

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
                RRC
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0b11100100,
                },
                Flags = new ConditionFlags()
                {
                    Carry = true,
                }
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b01110010, state.Registers.A);

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
