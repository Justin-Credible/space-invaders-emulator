using System;
using System.IO;
using System.Text;
using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class CPUIntegrationTest : BaseTest
    {
        private const int MAX_INSTRUCTIONS = 10000;

        [Fact]
        public void IntegrationTest()
        {
            var originalRom = File.ReadAllBytes("../../../CPUDiag/cpudiag.bin");

            // The assembled version is expected to be loaded at 0x100. So here
            // we prepend 256 empty bytes by copying into a new array.
            var rom = new byte[0x100 + originalRom.Length];
            Array.Copy(originalRom, 0, rom, 0x100, originalRom.Length);

            // Ensure we're running the CPU in a special diagnostics mode.
            // This allows for special behavior as if CP/M was running our
            // program (e.g. JMP 0x00 exits and CALL 0x05 prints a message).
            var cpu = new CPU()
            {
                EnableCPUDiagMode = true,

                // The cpudiag.bin program is assembled with it's first instruction at $100.
                ProgramCounter = 0x100,
            };

            cpu.LoadRom(rom);

            var passed = false;
            string consoleOutput = "";

            // This event handler will fire when CALL 0x05 is executed.
            // Based on the output string from the program we know if the test
            // suite passed or not.
            cpu.OnCPUDiagDebugEvent += (int eventID) =>
            {
                if (eventID == 2)
                {
                    // This appears to be a character printing routine using the value
                    // of the A register. On failures, this appends an exit code to the
                    // console output.
                    consoleOutput += (char)cpu.Registers.A;
                }
                else if (eventID == 9)
                {
                    // Apparently the string to be printed by CP/M is pointed at by
                    // the address stored in the DE register pair. Here we fetch it.
                    consoleOutput = FetchString(cpu, cpu.Registers.DE);
                    Console.WriteLine(consoleOutput);

                    // The cpudiag.bin program prints this text if the tests pass.
                    if (consoleOutput.Contains("CPU IS OPERATIONAL"))
                        passed = true;
                }
            };

            // Patch the program as per this page:
            // http://www.emulator101.com/full-8080-emulation.html

            // Fix the stack pointer from 0x6ad to 0x7ad
            // this 0x06 byte 112 in the code, which is
            // byte 112 + 0x100 = 368 in memory
            cpu.Memory[368] = 0x7;

            // Skip DAA test (not implemented).
            cpu.Memory[0x59c] = 0xc3; //JMP
            cpu.Memory[0x59d] = 0xc2;
            cpu.Memory[0x59e] = 0x05;

            // Keep track of how many instructions we've executed so we can
            // attempt to detect an infinite loop or other anomalies.
            var instructionCount = 0;

            // Now run the program!
            while (instructionCount < MAX_INSTRUCTIONS)
            {
                cpu.Step();
                instructionCount++;

                if (cpu.Finished)
                    break;
            }

            Assert.True(instructionCount < MAX_INSTRUCTIONS, "Sanity check to ensure we didn't get in an infinite loop.");
            Assert.True(cpu.Finished);
            Assert.True(passed, $"cpudiag.bin console output was: '{consoleOutput}'");
        }

        private string FetchString(CPU cpu, UInt16 ptr)
        {
            var sb = new StringBuilder();

            while (true)
            {
                var c = (char)cpu.Memory[ptr];

                // The cpudiag.bin program terminates strings with the dollar sign.
                if (c == '$')
                    break;
                else
                    sb.Append(c);

                ptr++;
            }

            var result = sb.ToString();

            // \f\r\n Seems to be the prefix to all assembled strings?
            // We'll just strip them out here.
            if (result.StartsWith("\f\r\n"))
                result = result.Substring(3);

            return result;
        }
    }
}
