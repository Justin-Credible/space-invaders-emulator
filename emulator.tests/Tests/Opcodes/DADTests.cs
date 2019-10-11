using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class DADTests : BaseTest
    {
        [Theory]
        [InlineData(RegisterPair.BC)]
        [InlineData(RegisterPair.DE)]
        public void TestDAD_NoCarry(RegisterPair pair)
        {
            var rom = AssembleSource($@"
                org 00h
                DAD {pair.GetUpperRegister()}
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x1212,
                [pair] = 0x3434,
            };

            var flags = new ConditionFlags()
            {
                Zero = true,
                Sign = true,
                Parity = true,
                Carry = false,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x3434, state.Registers[pair]);
            Assert.Equal(0x4646, state.Registers.HL);

            // Ensure these flags remain unchanged.
            Assert.True(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.True(state.Flags.Parity);

            // No carry in this case.
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestDAD_H_NoCarry()
        {
            var rom = AssembleSource($@"
                org 00h
                DAD H
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x1212,
            };

            var flags = new ConditionFlags()
            {
                Zero = true,
                Sign = true,
                Parity = true,
                Carry = false,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x2424, state.Registers.HL);

            // Ensure these flags remain unchanged.
            Assert.True(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.True(state.Flags.Parity);

            // No carry in this case.
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestDAD_SP_NoCarry()
        {
            var rom = AssembleSource($@"
                org 00h
                DAD SP
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0x1212,
            };

            var flags = new ConditionFlags()
            {
                Zero = true,
                Sign = true,
                Parity = true,
                Carry = false,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                StackPointer = 0x3434,
                Flags = flags,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x3434, state.StackPointer);
            Assert.Equal(0x4646, state.Registers.HL);

            // Ensure these flags remain unchanged.
            Assert.True(state.Flags.Zero);
            Assert.True(state.Flags.Sign);
            Assert.True(state.Flags.Parity);

            // No carry in this case.
            Assert.False(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [InlineData(RegisterPair.BC)]
        [InlineData(RegisterPair.DE)]
        public void TestDAD_Carry(RegisterPair pair)
        {
            var rom = AssembleSource($@"
                org 00h
                DAD {pair.GetUpperRegister()}
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0xFFFE,
                [pair] = 0x0005,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x0005, state.Registers[pair]);
            Assert.Equal(0x0003, state.Registers.HL);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestDAD_HL_Carry()
        {
            var rom = AssembleSource($@"
                org 00h
                DAD H
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0xFFF0,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0xFFE0, state.Registers.HL);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestDAD_SP_Carry()
        {
            var rom = AssembleSource($@"
                org 00h
                DAD SP
                HLT
            ");

            var registers = new CPURegisters()
            {
                HL = 0xFFFE,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                StackPointer = 0x0005,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x0005, state.StackPointer);
            Assert.Equal(0x0003, state.Registers.HL);

            Assert.False(state.Flags.Zero);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
