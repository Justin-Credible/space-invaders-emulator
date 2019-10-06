using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class CALLTests : BaseTest
    {
        [Fact]
        public void TestCALL()
        {
            var rom = AssembleSource($@"
                org 00h
                NOP         ; $0000
                NOP         ; $0001
                CALL 000Ah  ; $0002
                NOP         ; $0005
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
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x271E, state.StackPointer);
            Assert.Equal(0xFF, state.Memory[0x2720]);
            Assert.Equal(0x00, state.Memory[0x271F]);
            Assert.Equal(0x02, state.Memory[0x271E]);
            Assert.Equal(0xFF, state.Memory[0x271D]);

            AssertFlagsFalse(state);

            Assert.Equal(4, state.Iterations);
            Assert.Equal(7 + (4*2) + 17, state.Cycles);
            Assert.Equal(0x000A, state.ProgramCounter);
        }

        [Fact]
        public void TestCALLAndRET()
        {
            var rom = AssembleSource($@"
                org 00h
                LXI SP,2720h; $0000 - $0002
                NOP         ; $0003
                CALL 0011h  ; $0004 - $0006
                CALL 0014h  ; $0007 - $0009
                CALL 0017h  ; $000A - $000C
                HLT         ; $000D
                NOP         ; $000E
                MVI H, 55h  ; $000F - $0010
                MVI A, 42h  ; $0011 - $0012
                RET         ; $0013
                MVI B, 66h  ; $0014 - $0015
                RET         ; $0016
                MVI C, 77h  ; $0017 - $0018
                MVI D, 88h  ; $0019 - $001A
                RET         ; $001B
                MVI E, 99h  ; $001C - $001D
            ");

            var memory = new byte[16384];
            memory[0x2720] = 0xFF;
            memory[0x271F] = 0xFF;
            memory[0x271E] = 0xFF;
            memory[0x271D] = 0xFF;

            var initialState = new InitialCPUState()
            {
                Memory = memory,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x2720, state.StackPointer);
            Assert.Equal(0xFF, state.Memory[0x2720]);
            Assert.Equal(0x00, state.Memory[0x271F]);
            Assert.Equal(0x0A, state.Memory[0x271E]);
            Assert.Equal(0xFF, state.Memory[0x271D]);

            AssertFlagsFalse(state);

            Assert.Equal(13, state.Iterations);
            Assert.Equal(7 + 10 + 4 + (3*17) + (3*10) + (4*7), state.Cycles);
            Assert.Equal(0x000D, state.ProgramCounter);
        }
    }
}
