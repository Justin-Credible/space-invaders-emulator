using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class INTEsts : BaseTest
    {
        [Fact]
        public void TestIN()
        {
            var rom = AssembleSource($@"
                org 00h
                IN 2
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0x00,
                },
            };

            var cpu = new CPU();

            cpu.OnDeviceRead += (int deviceID) => {

                if (deviceID == 1) {
                    return 0x77;
                }
                else if (deviceID == 2) {
                    return 0x66;
                }
                else {
                    return 0x55;
                }
            };

            var state = Execute(rom, initialState, cpu);

            Assert.Equal(0x66, state.Registers.A);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestIN_DoesNotThrowExceptionIfNoHandlersPresent()
        {
            var rom = AssembleSource($@"
                org 00h
                IN 2
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0x11,
                },
            };

            var cpu = new CPU();

            var state = Execute(rom, initialState, cpu);

            Assert.Equal(0x11, state.Registers.A);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }
    }
}
