using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class InstructionTests : BaseTest
    {
        [Fact]
        public void TestMOV()
        {
            var rom = AssembleSource(@"
                org 00h
                MOV B, D
                HLT
            ");

            var initialState = new CPUState()
            {
                Registers = new Registers()
                {
                    D = 42,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(state.Registers.B, 42);
            Assert.Equal(state.Registers.D, 42);
            Assert.Equal(2, state.Iterations);
            Assert.Equal(5 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
