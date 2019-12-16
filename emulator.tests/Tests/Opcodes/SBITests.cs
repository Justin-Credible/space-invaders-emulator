using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class SBITests : BaseTest
    {
        [Fact]
        public void TestSBI_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                SBI 18h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x42,
                },
                Flags = new ConditionFlags()
                {
                    Carry = true,
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
        public void TestSBI_DoesNotSubtractOneIfCarryFalse()
        {
            var rom = AssembleSource($@"
                org 00h
                SBI 19h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x42,
                },
                Flags = new ConditionFlags()
                {
                    Carry = false,
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
        public void TestSBI_ParityFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                SBI 16h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x42,
                },
                Flags = new ConditionFlags()
                {
                    Carry = true,
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
        public void TestSBI_SignFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                SBI 15h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0xFF,
                },
                Flags = new ConditionFlags()
                {
                    Carry = true,
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
        public void TestSBI_CarryFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                SBI 06h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x02,
                },
                Flags = new ConditionFlags()
                {
                    Carry = true,
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
        public void TestSBI_ZeroFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                SBI 11h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0x12,
                },
                Flags = new ConditionFlags()
                {
                    Carry = true,
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
