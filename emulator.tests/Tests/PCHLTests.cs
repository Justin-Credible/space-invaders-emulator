using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class PCHLTests : BaseTest
    {
        [Fact]
        public void TestPCHL()
        {
            var rom = AssembleSource($@"
                org 00h
                PCHL    ; $0000
                NOP     ; $0001
                NOP     ; $0002
                HLT     ; $0003
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    HL = 0x003,
                },
            };

            var state = Execute(rom, initialState);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 5, state.Cycles);
            Assert.Equal(0x0003, state.ProgramCounter);
        }
    }
}
