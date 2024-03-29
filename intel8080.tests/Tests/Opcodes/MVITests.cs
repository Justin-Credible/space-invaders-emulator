using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class MVITests : BaseTest
    {
        [Theory]
        [ClassData(typeof(RegistersClassData))]
        public void TestMVIToRegister(Register destReg)
        {
            var rom = AssembleSource($@"
                org 00h
                MVI {destReg}, 42h
                HLT
            ");

            var state = Execute(rom);

            Assert.Equal(0x42, state.Registers[destReg]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestMVIToMemory()
        {
            var rom = AssembleSource($@"
                org 00h
                MVI M, 42h
                HLT
            ");

            var registers = new CPURegisters();
            registers[Register.H] = 0x22;
            registers[Register.L] = 0x33;

            var initialState = new CPUConfig()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(0x42, state.Memory[0x2233]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(10 + 7, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }
    }
}
