using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class SUITests : BaseTest
    {
        [Fact]
        public void TestSUI_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                SUI 19h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x42,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x29, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestSUI_ParityFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                SUI 17h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x42,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x2B, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestSUI_SignFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                SUI 16h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0xFF,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0xE9, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestSUI_CarryFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                SUI 07h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x02,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0xFB, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestSUI_ZeroFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                SUI 12h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x12,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x00, state.Registers.A);

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
