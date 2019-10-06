using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class JMTests : BaseTest
    {
        [Fact]
        public void TestJM_Jumps()
        {
            var rom = AssembleSource($@"
                org 00h
                NOP         ; $0000
                NOP         ; $0001
                JM 000Ah    ; $0002
                HLT         ; $0005
                NOP         ; $0006
                NOP         ; $0007
                NOP         ; $0008
                NOP         ; $0009
                HLT         ; $000A
            ");

            var initialState = new InitialCPUState()
            {
                Flags = new ConditionFlags()
                {
                    Sign = true,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x0000, state.StackPointer);

            Assert.False(state.Flags.Carry);
            Assert.False(state.Flags.Parity);
            Assert.True(state.Flags.Sign);
            Assert.False(state.Flags.Zero);

            Assert.Equal(4, state.Iterations);
            Assert.Equal(7 + (4*2) + 10, state.Cycles);
            Assert.Equal(0x000A, state.ProgramCounter);
        }

        [Fact]
        public void TestJM_DoesNotJump()
        {
            var rom = AssembleSource($@"
                org 00h
                NOP         ; $0000
                NOP         ; $0001
                JM 000Ah    ; $0002
                HLT         ; $0005
                NOP         ; $0006
                NOP         ; $0007
                NOP         ; $0008
                NOP         ; $0009
                HLT         ; $000A
            ");

            var initialState = new InitialCPUState()
            {
                Flags = new ConditionFlags()
                {
                    Sign = false,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x0000, state.StackPointer);

            Assert.False(state.Flags.Carry);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Zero);

            Assert.Equal(4, state.Iterations);
            Assert.Equal(7 + (4*2) + 10, state.Cycles);
            Assert.Equal(0x0005, state.ProgramCounter);
        }
    }
}
