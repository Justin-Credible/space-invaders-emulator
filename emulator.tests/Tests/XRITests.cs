using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class XRITests : BaseTest
    {
        [Fact]
        public void TestXRI_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                XRI 10110001B
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b11010101,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b01100100, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestXRI_ParityFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                XRI 01001001B
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b01101101,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b00100100, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestXRI_SignFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                XRI 11001001B
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b01101101,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0b10100100, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestXRI_ZeroFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                XRI 11001001B
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0b11001001,
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
