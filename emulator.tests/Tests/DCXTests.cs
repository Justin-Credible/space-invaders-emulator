using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class DCXTests : BaseTest
    {
        [Theory]
        [InlineData(RegisterPair.BC)]
        [InlineData(RegisterPair.DE)]
        [InlineData(RegisterPair.HL)]
        public void TestDCX(RegisterPair pair)
        {
            var rom = AssembleSource($@"
                org 00h
                DCX {pair.GetUpperRegister()}
                DCX {pair.GetUpperRegister()}
                DCX {pair.GetUpperRegister()}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [pair] = 0x3902,
            };

            var initialState = new InitialCPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x38FF, state.Registers[pair]);

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
