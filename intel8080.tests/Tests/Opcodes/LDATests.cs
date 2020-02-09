using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class LDATests : BaseTest
    {
        [Fact]
        public void TestLDA()
        {
            var rom = AssembleSource($@"
                org 00h
                LDA 2477h
                HLT
            ");

            var memory = new byte[16384];
            memory[0x2477] = 0x42;

            var initialState = new CPUConfig()
            {
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x42, state.Memory[0x2477]);
            Assert.Equal(0x42, state.Registers.A);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 13, state.Cycles);
            Assert.Equal(0x03, state.ProgramCounter);
        }
    }
}
