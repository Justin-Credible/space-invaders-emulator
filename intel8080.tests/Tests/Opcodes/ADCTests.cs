using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class ADCTests : BaseTest
    {
        [Theory]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestADC_NoFlags(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                ADC {sourceReg}
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x42;
            registers[sourceReg] = 0x15;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x58, state.Registers.A);
            Assert.Equal(0x15, state.Registers[sourceReg]);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestADC_CarryFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                ADC {sourceReg}
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0xFE;
            registers[sourceReg] = 0x03;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x02, state.Registers.A);
            Assert.Equal(0x03, state.Registers[sourceReg]);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestADC_ZeroFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                ADC {sourceReg}
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0xFE;
            registers[sourceReg] = 0x01;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x00, state.Registers.A);
            Assert.Equal(0x01, state.Registers[sourceReg]);

            Assert.True(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestADC_ParityFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                ADC {sourceReg}
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x44;
            registers[sourceReg] = 0x32;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x77, state.Registers.A);
            Assert.Equal(0x32, state.Registers[sourceReg]);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [InlineData(Register.B)]
        [InlineData(Register.C)]
        [InlineData(Register.D)]
        [InlineData(Register.E)]
        [InlineData(Register.H)]
        [InlineData(Register.L)]
        public void TestADC_SignFlag(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                ADC {sourceReg}
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x4D;
            registers[sourceReg] = 0x39;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x87, state.Registers.A);
            Assert.Equal(0x39, state.Registers[sourceReg]);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestADC_A_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                ADC A
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x03;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x07, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestADC_A_CarryFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                ADC A
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x80;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x01, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestADC_A_SignFlag()
        {
            var rom = AssembleSource($@"
                org 00h
                ADC A
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x44;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
            };
            var state = Execute(rom, initialState);

            Assert.Equal(0x89, state.Registers.A);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 4, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestADC_M_NoFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                ADC M
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x42;
            registers.HL = 0x2477;

            var memory = new byte[16384];
            memory[0x2477] = 0x15;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x58, state.Registers.A);
            Assert.Equal(0x15, state.Memory[0x2477]);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestADC_M_ZeroAndCarryFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                ADC M
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0xC0;
            registers.HL = 0x2477;

            var memory = new byte[16384];
            memory[0x2477] = 0x3F;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x00, state.Registers.A);
            Assert.Equal(0x3F, state.Memory[0x2477]);

            Assert.True(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestADC_M_SignAndParityFlags()
        {
            var rom = AssembleSource($@"
                org 00h
                ADC M
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x47;
            registers.HL = 0x2477;

            var memory = new byte[16384];
            memory[0x2477] = 0x43;

            var flags = new ConditionFlags()
            {
                Carry = true,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
                Flags = flags,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x8B, state.Registers.A);
            Assert.Equal(0x43, state.Memory[0x2477]);

            Assert.False(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.True(state.Flags.Parity);
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
