using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class LXITests : BaseTest
    {
        [Theory]
        [InlineData(RegisterID.B, RegisterID.C)]
        [InlineData(RegisterID.D, RegisterID.E)]
        [InlineData(RegisterID.H, RegisterID.L)]
        public void TestMVI(RegisterID destReg, RegisterID destReg2)
        {
            var rom = AssembleSource($@"
                org 00h
                LXI {destReg}, 4277h
                HLT
            ");

            var state = Execute(rom);

            Assert.Equal(0x42, state.Registers[destReg]);
            Assert.Equal(0x77, state.Registers[destReg2]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(10 + 7, state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }

        [Fact]
        public void TestLXIToStackPointer()
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
