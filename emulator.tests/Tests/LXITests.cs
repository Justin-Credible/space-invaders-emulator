using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class LXITests : BaseTest
    {
        [Theory]
        [InlineData(RegisterPair.BC)]
        [InlineData(RegisterPair.DE)]
        [InlineData(RegisterPair.HL)]
        public void TestLXI(RegisterPair pair)
        {
            var rom = AssembleSource($@"
                org 00h
                LXI {pair.GetUpperRegister()}, 4277h
                HLT
            ");

            var state = Execute(rom);

            Assert.Equal(0x4277, state.Registers[pair]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(10 + 7, state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }

        [Fact]
        public void TestLXI_SP()
        {
            var rom = AssembleSource($@"
                org 00h
                LXI SP, 4277h
                HLT
            ");

            var state = Execute(rom);

            Assert.Equal(0x4277, state.StackPointer);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(10 + 7, state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }
    }
}
