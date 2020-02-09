using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class CMATests : BaseTest
    {
        [Fact]
        public void TestCMA_DoesNotSetParityOrSignFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                CMA
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0b01011010,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b10100101, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestCMA_DoesNotSetZeroFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                CMA
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0b11111111,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b00000000, state.Registers.A);

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
