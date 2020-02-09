using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class MOVTests : BaseTest
    {
        [Theory]
        [ClassData(typeof(RegisterPermutationsClassData))]
        public void TestMOVFromRegisterToRegister(Register destReg, Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                MOV {destReg}, {sourceReg}
                HLT
            ");

            var registers = new CPURegisters();
            registers[sourceReg] = 42;

            var initialState = new CPUConfig()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(42, state.Registers[destReg]);
            Assert.Equal(42, state.Registers[sourceReg]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(5 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [ClassData(typeof(RegistersClassData))]
        public void TestMOVFromRegisterToMemory(Register sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                MOV M, {sourceReg}
                HLT
            ");

            var registers = new CPURegisters();

            if (sourceReg != Register.H && sourceReg != Register.L)
                registers[sourceReg] = 0x77;

            registers[Register.H] = 0x21;
            registers[Register.L] = 0x35;

            var initialState = new CPUConfig()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            if (sourceReg == Register.H)
            {
                Assert.Equal(0x21, state.Memory[0x2135]);
            }
            else if (sourceReg == Register.L)
            {
                Assert.Equal(0x35, state.Memory[0x2135]);
            }
            else
            {
                Assert.Equal(0x77, state.Memory[0x2135]);
                Assert.Equal(0x77, state.Registers[sourceReg]);
            }

            // Address registers should remain unmodified.
            Assert.Equal(0x21, state.Registers[Register.H]);
            Assert.Equal(0x35, state.Registers[Register.L]);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [ClassData(typeof(RegistersClassData))]
        public void TestMOVFromMemoryToRegister(Register destReg)
        {
            var rom = AssembleSource($@"
                org 00h
                MOV {destReg}, M
                HLT
            ");

            var registers = new CPURegisters();

            registers[Register.H] = 0x21;
            registers[Register.L] = 0x35;

            var memory = new byte[16384];
            memory[0x2135] = 0x42;

            var initialState = new CPUConfig()
            {
                Registers = registers,
                MemorySize = memory.Length,
            };

            var state = Execute(rom, memory, initialState);

            // Memory should remain unmodified.
            Assert.Equal(0x42, state.Memory[0x2135]);

            if (destReg == Register.H)
            {
                Assert.Equal(0x42, state.Registers[Register.H]);
                Assert.Equal(0x35, state.Registers[Register.L]);
            }
            else if (destReg == Register.L)
            {
                Assert.Equal(0x21, state.Registers[Register.H]);
                Assert.Equal(0x42, state.Registers[Register.L]);
            }
            else
            {
                // Value from memory pointed at by HL should have been loaded into the destination register.
                Assert.Equal(0x42, state.Registers[destReg]);

                // Address registers should remain unmodified.
                Assert.Equal(0x21, state.Registers[Register.H]);
                Assert.Equal(0x35, state.Registers[Register.L]);
            }

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
