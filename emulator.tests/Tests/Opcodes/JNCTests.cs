using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class JNCTests : BaseTest
    {
        [Fact]
        public void TestJNC_Jumps()
        {
            var rom = AssembleSource($@"
                org 00h
                NOP         ; $0000
                NOP         ; $0001
                JNC 000Ah   ; $0002
                HLT         ; $0005
                NOP         ; $0006
                NOP         ; $0007
                NOP         ; $0008
                NOP         ; $0009
                HLT         ; $000A
            ");

            var initialState = new CPUConfig()
            {
                Flags = new ConditionFlags()
                {
                    Carry = false,
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
            Assert.Equal(0x000A, state.ProgramCounter);
        }

        [Fact]
        public void TestJNC_DoesNotJump()
        {
            var rom = AssembleSource($@"
                org 00h
                NOP         ; $0000
                NOP         ; $0001
                JNC 000Ah   ; $0002
                HLT         ; $0005
                NOP         ; $0006
                NOP         ; $0007
                NOP         ; $0008
                NOP         ; $0009
                HLT         ; $000A
            ");

            var initialState = new CPUConfig()
            {
                Flags = new ConditionFlags()
                {
                    Carry = true,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x0000, state.StackPointer);

            Assert.True(state.Flags.Carry);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Zero);

            Assert.Equal(4, state.Iterations);
            Assert.Equal(7 + (4*2) + 10, state.Cycles);
            Assert.Equal(0x0005, state.ProgramCounter);
        }
    }
}
