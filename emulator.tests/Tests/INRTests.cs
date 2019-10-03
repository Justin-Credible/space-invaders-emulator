using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class INRTests : BaseTest
    {
        [Theory]
        [InlineData(Register.A)]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestINR_NoFlags(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                INR {sourceReg}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [sourceReg] = 0x42,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x43, state.Registers[sourceReg]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 5, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [InlineData(Register.A)]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestINR_ParityFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                INR {sourceReg}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [sourceReg] = 0x43,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x44, state.Registers[sourceReg]);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 5, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [InlineData(Register.A)]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestINR_SignFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                INR {sourceReg}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [sourceReg] = 0x7F,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x80, state.Registers[sourceReg]);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 5, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [InlineData(Register.A)]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestINR_ZeroButNoCarryFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                INR {sourceReg}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [sourceReg] = 0xFF,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x00, state.Registers[sourceReg]);

            Assert.True(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 5, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestINR_M_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                INR M
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477] = 0x42;

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                Memory = memory,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x43, state.Memory[0x2477]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestINR_M_ParityFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                INR M
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477] = 0x43;

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                Memory = memory,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x44, state.Memory[0x2477]);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestINR_M_SignFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                INR M
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477] = 0x7F;

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                Memory = memory,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x80, state.Memory[0x2477]);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestINR_M_ZeroButNoCarryFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                INR M
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477] = 0xFF;

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                Memory = memory,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x00, state.Memory[0x2477]);

            Assert.True(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
