using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class CNCTests : BaseTest
    {
        [Fact]
        public void TestCNC_Calls()
        {
            var rom = AssembleSource($@"
                org 00h
                NOP         ; $0000
                NOP         ; $0001
                CNC 000Ah   ; $0002
                HLT         ; $0005
                NOP         ; $0006
                NOP         ; $0007
                NOP         ; $0008
                NOP         ; $0009
                HLT         ; $000A
            ");

            var memory = new byte[16384];
            memory[0x2720] = 0xFF;
            memory[0x271F] = 0xFF;
            memory[0x271E] = 0xFF;
            memory[0x271D] = 0xFF;

            var initialState = new InitialCPUState()
            {
                StackPointer = 0x2720,
                Memory = memory,
                Flags = new ConditionFlags()
                {
                    Carry = false,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x271E, state.StackPointer);
            Assert.Equal(0xFF, state.Memory[0x2720]);
            Assert.Equal(0x00, state.Memory[0x271F]);
            Assert.Equal(0x02, state.Memory[0x271E]);
            Assert.Equal(0xFF, state.Memory[0x271D]);

            Assert.Equal(4, state.Iterations);
            Assert.Equal(7 + (4*2) + 17, state.Cycles);
            Assert.Equal(0x000A, state.ProgramCounter);
        }

        [Fact]
        public void TestCNC_DoesNotCall()
        {
            var rom = AssembleSource($@"
                org 00h
                NOP         ; $0000
                NOP         ; $0001
                CNC 000Ah   ; $0002
                HLT         ; $0005
                NOP         ; $0006
                NOP         ; $0007
                NOP         ; $0008
                NOP         ; $0009
                HLT         ; $000A
            ");

            var memory = new byte[16384];
            memory[0x2720] = 0xFF;
            memory[0x271F] = 0xFF;
            memory[0x271E] = 0xFF;
            memory[0x271D] = 0xFF;

            var initialState = new InitialCPUState()
            {
                StackPointer = 0x2720,
                Memory = memory,
                Flags = new ConditionFlags()
                {
                    Carry = true,
                },
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x2720, state.StackPointer);
            Assert.Equal(0xFF, state.Memory[0x2720]);
            Assert.Equal(0xFF, state.Memory[0x271F]);
            Assert.Equal(0xFF, state.Memory[0x271E]);
            Assert.Equal(0xFF, state.Memory[0x271D]);

            Assert.Equal(4, state.Iterations);
            Assert.Equal(7 + (4*2) + 11, state.Cycles);
            Assert.Equal(0x0005, state.ProgramCounter);
        }
    }
}
