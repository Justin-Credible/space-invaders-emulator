using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class STAXTests : BaseTest
    {
        [Theory]
        [InlineData(Register.B, Register.C)]
        [InlineData(Register.D, Register.E)]
        public void TestSTAX(Register destReg, Register destReg2)
        {
            var rom = AssembleSource($@"
                org 00h
                STAX {destReg}
                HLT
            ");

            var registers = new CPURegisters();
            registers.A = 0x42;
            registers[destReg] = 0x24;
            registers[destReg2] = 0x77;

            var initialState = new CPUConfig()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x42, state.Memory[0x2477]);
            Assert.Equal(0x42, state.Registers.A);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
