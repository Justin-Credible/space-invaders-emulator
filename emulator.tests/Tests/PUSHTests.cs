using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class PushTests : BaseTest
    {
        [Theory]
        [InlineData(Register.B, Register.C)]
        [InlineData(Register.D, Register.E)]
        [InlineData(Register.H, Register.L)]
        public void TestPUSH(Register sourceReg, Register sourceReg2)
        {
            var rom = AssembleSource($@"
                org 00h
                PUSH {sourceReg}
                HLT
            ");

            var registers = new CPURegisters();
            registers[sourceReg] = 0x24;
            registers[sourceReg2] = 0x77;

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                StackPointer = 0x3000,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x24, state.Registers[sourceReg]);
            Assert.Equal(0x77, state.Registers[sourceReg2]);
            Assert.Equal(0x00, state.Memory[0x3000]);
            Assert.Equal(0x24, state.Memory[0x2FFF]);
            Assert.Equal(0x77, state.Memory[0x2FFE]);
            Assert.Equal(0x2FFE, state.StackPointer);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 11, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestPUSH_PSW()
        {
            var rom = AssembleSource($@"
                org 00h
                PUSH PSW
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x42;

            // 7 6 5 4 3 2 1 0
            // S Z 0 A 0 P 1 C
            // 1 1 0 1 0 1 1 1
            //       D 7
            var flags = new ConditionFlags()
            {
                Sign = true,
                Zero = true,
                AuxCarry = true,
                Parity = true,
                Carry = true,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                Flags = flags,
                StackPointer = 0x3000,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x42, state.Registers.A);
            Assert.Equal(0x00, state.Memory[0x3000]);
            Assert.Equal(0x42, state.Memory[0x2FFF]);
            Assert.Equal(0xD7, state.Memory[0x2FFE]);
            Assert.Equal(0x2FFE, state.StackPointer);

            Assert.True(state.Flags.Sign);
            Assert.True(state.Flags.Zero);
            Assert.True(state.Flags.AuxCarry);
            Assert.True(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 11, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
