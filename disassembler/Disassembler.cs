using System;
using System.Text;

namespace JustinCredible.I8080Disassembler
{
    public static class Disassembler
    {

        public static string Disassemble(byte[] rom, bool emitAddresses = false)
        {
            var disassembly = new StringBuilder();

            for (var i = 0; i < rom.Length; i++)
            {
                var instruction = Disassemble(rom, i, emitAddress);
                disassembly.AppendLine(instruction);
            }

            return disassembly.ToString();
        }

        public static string Disassemble(byte[] rom, UInt16 address, bool emitAddress = false)
        {

        }
    }
}
