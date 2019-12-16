using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class ACITests : BaseTest
    {
        [Fact]
        public void TestACI_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                ACI 18h
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

            Assert.Equal(0x5B, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestACI_DoesNotAddOneIfCarryFalse()
        {
            var rom = AssembleSource($@"
                org 00h
                ACI 19h
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

            Assert.Equal(0x5B, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestACI_ParityFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                ACI 19h
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

            Assert.Equal(0x5C, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestACI_SignFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                ACI 49h
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

            Assert.Equal(0x8C, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestACI_CarryFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                ACI 1Ch
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0xEE,
                },
                Flags = new ConditionFlags()
                {
                    Carry = true,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x0B, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestACI_ZeroFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                ACI 11h
                HLT
            ");

            var initialState = new CPUConfig()
            {
                Registers = new CPURegisters()
                {
                    A = 0xEE,
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
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }
    }
}
