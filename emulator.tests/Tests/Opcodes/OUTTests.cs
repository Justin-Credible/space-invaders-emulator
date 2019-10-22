using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class OUTTEsts : BaseTest
    {
        [Fact]
        public void TestOUT()
        {
            var rom = AssembleSource($@"
                org 00h
                OUT 3
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0x42,
                },
            };

            var cpu = new CPU();

            byte actualData = 0;
            var actualDeviceID = -1;

            cpu.OnDeviceWrite += (int deviceID, byte data) => {
                actualDeviceID = deviceID;
                actualData = data;
            };

            var state = Execute(rom, initialState, cpu);

            Assert.Equal(0x42, actualData);
            Assert.Equal(3, actualDeviceID);

            Assert.Equal(0x42, state.Registers.A);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }

        [Fact]
        public void TestOUT_DoesNotThrowExceptionIfNoHandlersPresent()
        {
            var rom = AssembleSource($@"
                org 00h
                OUT 3
                HLT
            ");

            var initialState = new InitialCPUState()
            {
                Registers = new CPURegisters()
                {
                    A = 0x42,
                },
            };

            var cpu = new CPU();

            var state = Execute(rom, initialState, cpu);

            Assert.Equal(0x42, state.Registers.A);

            AssertFlagsFalse(state);

            Assert.Equal(2, state.Iterations);
            Assert.Equal(7 + 10, state.Cycles);
            Assert.Equal(0x02, state.ProgramCounter);
        }
    }
}
