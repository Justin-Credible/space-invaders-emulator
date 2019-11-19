using System;
using System.IO;
using System.Text;
using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class CPUIntegrationTest : BaseTest
    {
        [Fact(Skip="Not passing yet.")]
        // [Fact]
        public void IntegrationTest()
        {
            var originalRom = File.ReadAllBytes("../../../CPUDiag/cpudiag.bin");

            // The assembled version is expected to be loaded at 0x100. So here
            // we prepend 256 empty bytes by copying into a new array.
            var rom = new byte[0x100 + originalRom.Length];
            Array.Copy(originalRom, 0, rom, 0x100, originalRom.Length);

            var cpu = new CPU();
            cpu.LoadRom(rom);

            var passed = false;

            cpu.OnCPUDiagDebugEvent += (int eventID) =>
            {
                if (eventID == 2)
                    Console.Write("Character output routine called!");
                else if (eventID == 9)
                {
                    var str = FetchString(cpu, cpu.Registers.DE);
                    Console.Write(str);

                    // The cpudiag.bin program prints this text if the tests pass.
                    if (str == " CPU IS OPERATIONAL")
                        passed = true;
                }
            };

            // Patch the program as per this page:
            // http://www.emulator101.com/full-8080-emulation.html

            // Fix the first instruction to be JMP 0x100
            cpu.Memory[0] = 0xc3;
            cpu.Memory[1] = 0;
            cpu.Memory[2] = 0x01;

            // Fix the stack pointer from 0x6ad to 0x7ad
            // this 0x06 byte 112 in the code, which is
            // byte 112 + 0x100 = 368 in memory
            cpu.Memory[368] = 0x7;

            // Skip DAA test
            cpu.Memory[0x59c] = 0xc3; //JMP
            cpu.Memory[0x59d] = 0xc2;
            cpu.Memory[0x59e] = 0x05;

            var instructionCount = 0;

            while (instructionCount < 10000)
            {
                cpu.Step();
                instructionCount++;

                if (cpu.Finished)
                    break;
            }

            Assert.True(instructionCount < 10000, "Sanity check to ensure we didn't get in an infinite loop.");
            Assert.True(cpu.Finished);
            Assert.True(passed);
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

            return sb.ToString();
        }
    }
}
