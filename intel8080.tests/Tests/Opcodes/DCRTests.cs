using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class DCRTests : BaseTest
    {
        [Theory]
        [InlineData(Register.A)]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestDCR_NoFlags(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                DCR {sourceReg}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [sourceReg] = 0x44,
            };

            var initialState = new CPUConfig()
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
        public void TestDCR_ParityFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                DCR {sourceReg}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [sourceReg] = 0x45,
            };

            var initialState = new CPUConfig()
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
        public void TestDCR_SignFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                DCR {sourceReg}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [sourceReg] = 0x81,
            };

            var initialState = new CPUConfig()
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
        public void TestDCR_ZeroFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                DCR {sourceReg}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [sourceReg] = 0x01,
            };

            var initialState = new CPUConfig()
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

        [Theory]
        [InlineData(Register.A)]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestDCR_NoCarryFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                DCR {sourceReg}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [sourceReg] = 0x00,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0xFF, state.Registers[sourceReg]);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 5, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestDCR_M_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                DCR M
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477] = 0x44;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x43, state.Memory[0x2477]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestDCR_M_ParityFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                DCR M
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477] = 0x45;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

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
        public void TestDCR_M_SignFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                DCR M
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477] = 0x81;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

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
        public void TestDCR_M_ZeroFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                DCR M
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477] = 0x01;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x00, state.Memory[0x2477]);

            Assert.True(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestDCR_M_NoCarryFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                DCR M
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x2477,
            };

            var memory = new byte[16384];
            memory[0x2477] = 0x00;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0xFF, state.Memory[0x2477]);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
