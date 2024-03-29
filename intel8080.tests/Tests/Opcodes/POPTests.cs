using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class POPTests : BaseTest
    {
        [Theory]
        [InlineData(RegisterPair.BC)]
        [InlineData(RegisterPair.DE)]
        [InlineData(RegisterPair.HL)]
        public void TestPop(RegisterPair pair)
        {
            var rom = AssembleSource($@"
                org 00h
                POP {pair.GetUpperRegister()}
                HLT
            ");

            var memory = new byte[16384];
            memory[0x2FFE] = 0x77;
            memory[0x2FFF] = 0x24;

            var initialState = new CPUConfig()
            {
                StackPointer = 0x2FFE,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x2477, state.Registers[pair]);
            Assert.Equal(0x00, state.Memory[0x3000]);
            Assert.Equal(0x24, state.Memory[0x2FFF]);
            Assert.Equal(0x77, state.Memory[0x2FFE]);
            Assert.Equal(0x3000, state.StackPointer);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Fact]
        public void TestPOP_PSW()
        {
            var rom = AssembleSource($@"
                org 00h
                POP PSW
                HLT
            ");

            var memory = new byte[16384];

            // 7 6 5 4 3 2 1 0
            // S Z 0 A 0 P 1 C
            // 1 1 0 1 0 1 1 1
            //       D 7
            memory[0x2FFE] = 0xD7;

            memory[0x2FFF] = 0x42;

            var registers = new CPURegisters();
            registers.A = 0x42;

            var initialState = new CPUConfig()
            {
                StackPointer = 0x2FFE,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            Assert.Equal(0x42, state.Registers.A);
            Assert.Equal(0x00, state.Memory[0x3000]);
            Assert.Equal(0x42, state.Memory[0x2FFF]);
            Assert.Equal(0xD7, state.Memory[0x2FFE]);
            Assert.Equal(0x3000, state.StackPointer);

            Assert.True(state.Flags.Sign);
            Assert.True(state.Flags.Zero);
            Assert.True(state.Flags.AuxCarry);
            Assert.True(state.Flags.Parity);
            Assert.True(state.Flags.Carry);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
