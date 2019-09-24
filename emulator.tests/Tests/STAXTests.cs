using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class STAXTests : BaseTest
    {
        [Theory]
        [InlineData(RegisterID.B, RegisterID.C)]
        [InlineData(RegisterID.D, RegisterID.E)]
        public void TestMVI(RegisterID destReg, RegisterID destReg2)
        {
            var rom = AssembleSource($@"
                org 00h
                STAX {destReg}
                HLT
            ");

            var registers = new Registers();
            registers.A = 0x42;
            registers[destReg] = 0x24;
            registers[destReg2] = 0x77;

            var initialState = new CPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x42, state.Memory[0x2477]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
