using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class MOVTests : BaseTest
    {
        [Theory]
        [ClassData(typeof(RegisterPermutationsClassData))]
        public void TestMOVFromRegisterToRegister(RegisterID destReg, RegisterID sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                MOV {destReg}, {sourceReg}
                HLT
            ");

            var registers = new Registers();
            registers[sourceReg] = 42;

            var initialState = new CPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            Assert.Equal(42, state.Registers[destReg]);
            Assert.Equal(42, state.Registers[sourceReg]);

            Assert.False(state.Flags.AuxCarry);
            Assert.False(state.Flags.Carry);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Zero);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(5 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [ClassData(typeof(RegistersClassData))]
        public void TestMOVFromRegisterToMemory(RegisterID sourceReg)
        {
            var rom = AssembleSource($@"
                org 00h
                MOV M, {sourceReg}
                HLT
            ");

            var registers = new Registers();

            if (sourceReg != RegisterID.H && sourceReg != RegisterID.L)
                registers[sourceReg] = 0x77;

            registers[RegisterID.H] = 0x21;
            registers[RegisterID.L] = 0x35;

            var initialState = new CPUState()
            {
                Registers = registers,
            };

            var state = Execute(rom, initialState);

            if (sourceReg == RegisterID.H)
            {
                Assert.Equal(0x21, state.Memory[0x2135]);
            }
            else if (sourceReg == RegisterID.L)
            {
                Assert.Equal(0x35, state.Memory[0x2135]);
            }
            else
            {
                Assert.Equal(0x77, state.Memory[0x2135]);
                Assert.Equal(0x77, state.Registers[sourceReg]);
            }

            // Address registers should remain unmodified.
            Assert.Equal(0x21, state.Registers[RegisterID.H]);
            Assert.Equal(0x35, state.Registers[RegisterID.L]);

            Assert.False(state.Flags.AuxCarry);
            Assert.False(state.Flags.Carry);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Zero);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }

        [Theory]
        [ClassData(typeof(RegistersClassData))]
        public void TestMOVFromMemoryToRegister(RegisterID destReg)
        {
            var rom = AssembleSource($@"
                org 00h
                MOV {destReg}, M
                HLT
            ");

            var registers = new Registers();

            registers[RegisterID.H] = 0x21;
            registers[RegisterID.L] = 0x35;

            var memory = new byte[16384];
            memory[0x2135] = 0x42;

            var initialState = new CPUState()
            {
                Registers = registers,
                Memory = memory,
            };

            var state = Execute(rom, initialState);

            // Memory should remain unmodified.
            Assert.Equal(0x42, state.Memory[0x2135]);

            if (destReg == RegisterID.H)
            {
                Assert.Equal(0x42, state.Registers[RegisterID.H]);
                Assert.Equal(0x35, state.Registers[RegisterID.L]);
            }
            else if (destReg == RegisterID.L)
            {
                Assert.Equal(0x21, state.Registers[RegisterID.H]);
                Assert.Equal(0x42, state.Registers[RegisterID.L]);
            }
            else
            {
                // Value from memory pointed at by HL should have been loaded into the destination register.
                Assert.Equal(0x42, state.Registers[destReg]);

                // Address registers should remain unmodified.
                Assert.Equal(0x21, state.Registers[RegisterID.H]);
                Assert.Equal(0x35, state.Registers[RegisterID.L]);
            }

            Assert.False(state.Flags.AuxCarry);
            Assert.False(state.Flags.Carry);
            Assert.False(state.Flags.Parity);
            Assert.False(state.Flags.Sign);
            Assert.False(state.Flags.Zero);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 7, state.Cycles);
            Assert.Equal(0x01, state.ProgramCounter);
        }
    }
}
