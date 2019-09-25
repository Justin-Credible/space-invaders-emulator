using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class DCXTests : BaseTest
    {
        [Theory]
        [InlineData(Register.B, Register.C)]
        [InlineData(Register.D, Register.E)]
        [InlineData(Register.H, Register.L)]
        public void TestDCX(Register reg1, Register reg2)
        {
            var rom = AssembleSource($@"
                org 00h
                DCX {reg1}
                DCX {reg1}
                DCX {reg1}
                HLT
            ");

            var registers = new CPURegisters();
            registers[reg1] = 0x39;
            registers[reg2] = 0x02;

            var initialState = new InitialCPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x38, state.Registers[reg1]);
            Assert.Equal(0xFF, state.Registers[reg2]);

            AssertFlagsFalse(state);

            Assert.Equal(4, state.Iterations);
            Assert.Equal(7 + (5*3), state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }

        [Fact]
        public void TestINX_SP()
        {
            var rom = AssembleSource($@"
                org 00h
                DCX SP
                DCX SP
                DCX SP
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                StackPointer = 0x3902,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x38FF, state.StackPointer);

            AssertFlagsFalse(state);

            Assert.Equal(4, state.Iterations);
            Assert.Equal(7 + (5*3), state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }
    }
}
