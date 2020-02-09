using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class RETTests : BaseTest
    {
        [Fact]
        public void TestRET()
        {
            var rom = AssembleSource($@"
                org 00h
                NOP         ; $0000
                NOP         ; $0001
                NOP         ; $0002
                HLT         ; $0003
                HLT         ; $0004
                HLT         ; $0005
                NOP         ; $0006
                RET         ; $0007
                NOP         ; $0008
                NOP         ; $0009
                HLT         ; $000A
            ");

            var memory = new byte[16384];
            memory[0x2720] = 0xFF;
            memory[0x271F] = 0x00;
            memory[0x271E] = 0x02;
            memory[0x271D] = 0xFF;

            var initialState = new CPUConfig()
            {
                ProgramCounter = 0x0007,
                StackPointer = 0x271E,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x2720, state.StackPointer);
            Assert.Equal(0xFF, state.Memory[0x2720]);
            Assert.Equal(0x00, state.Memory[0x271F]);
            Assert.Equal(0x02, state.Memory[0x271E]);
            Assert.Equal(0xFF, state.Memory[0x271D]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x0003, state.ProgramCounter);
        }

        [Theory]
        [InlineData("CALL")]
        [InlineData("CM")]
        [InlineData("CPE")]
        [InlineData("CC")]
        [InlineData("CZ")]
        [InlineData("CP")]
        [InlineData("CPO")]
        [InlineData("CNC")]
        [InlineData("CNZ")]
        public void TestRET_AllowsReturnToAllCallOpcodes(string callOpcode)
        {
            var rom = AssembleSource($@"
                org 00h
                NOP         ; $0000
                NOP         ; $0001
                {callOpcode} 0009H ; $0002
                HLT         ; $0005
                HLT         ; $0006
                HLT         ; $0007
                NOP         ; $0008
                RET         ; $0009
                NOP         ; $000A
                NOP         ; $000B
                HLT         ; $000C
            ");

            var memory = new byte[16384];
            memory[0x2720] = 0xFF;
            memory[0x271F] = 0x00;
            memory[0x271E] = 0x02;
            memory[0x271D] = 0xFF;

            var initialState = new CPUConfig()
            {
                ProgramCounter = 0x0009,
                StackPointer = 0x271E,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x2720, state.StackPointer);
            Assert.Equal(0xFF, state.Memory[0x2720]);
            Assert.Equal(0x00, state.Memory[0x271F]);
            Assert.Equal(0x02, state.Memory[0x271E]);
            Assert.Equal(0xFF, state.Memory[0x271D]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x0005, state.ProgramCounter);
        }
    }
}
