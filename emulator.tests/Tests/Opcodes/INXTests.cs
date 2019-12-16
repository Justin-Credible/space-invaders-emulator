using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class INXTests : BaseTest
    {
        [Theory]
        [InlineData(RegisterPair.BC)]
        [InlineData(RegisterPair.DE)]
        [InlineData(RegisterPair.HL)]
        public void TestINX(RegisterPair pair)
        {
            var rom = AssembleSource($@"
                org 00h
                INX {pair.GetUpperRegister()}
                INX {pair.GetUpperRegister()}
                INX {pair.GetUpperRegister()}
                HLT
            ");

            var registers = new CPURegisters()
            {
                [pair] = 0x38FF,
            };

            var initialState = new CPUConfig()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x3902, state.Registers[pair]);

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
                INX SP
                INX SP
                INX SP
                HLT
            ");

            var initialState = new CPUConfig()
            {
                StackPointer = 0x38FF,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x3902, state.StackPointer);

            AssertFlagsFalse(state);

            Assert.Equal(4, state.Iterations);
            Assert.Equal(7 + (5*3), state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }
    }
}
