using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class LDAXTests : BaseTest
    {
        [Theory]
        [InlineData(Register.B, Register.C)]
        [InlineData(Register.D, Register.E)]
        public void TestLDAX(Register addressReg1, Register addressReg2)
        {
            var rom = AssembleSource($@"
                org 00h
                LDAX {addressReg1}
                HLT
            ");

            var registers = new CPURegisters();
            registers[addressReg1] = 0x24;
            registers[addressReg2] = 0x77;

            var memory = new byte[16384];
            memory[0x2477] = 0x42;

            var initialState = new InitialCPUState()
            {
                Registers = registers,
                Memory = memory,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x42, state.Memory[0x2477]);
            Assert.Equal(0x42, state.Registers.A);
            Assert.Equal(0x24, state.Registers[addressReg1]);
            Assert.Equal(0x77, state.Registers[addressReg2]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
