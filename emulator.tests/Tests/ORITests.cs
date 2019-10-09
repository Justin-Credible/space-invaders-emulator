using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class ORITests : BaseTest
    {
        [Fact]
        public void TestORI_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                ORI 01101100B
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b01100101,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b01101101, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestORI_ParityFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                ORI 00101010B
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b01100101,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b01101111, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestORI_SignFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                ORI 10100100B
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b01100101,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b11100101, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestORI_ZeroFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                ORI 00000000B
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b00000000,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b00000000, state.Registers.A);

            Assert.True(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }
    }
}
