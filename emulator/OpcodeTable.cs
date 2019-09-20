
using System.Collections.Generic;

namespace JustinCredible.SIEmulator
{
    /**
     * A lookup table for opcodes, which includes opcode sizes and cycle counts.
     * Derived from: http://www.pastraiser.com/cpu/i8080/i8080_opcodes.html
     */
    public class OpcodeTable
    {
        public static Opcode NOP = new Opcode(OpcodeBytes.NOP, 1, "NOP", 4);
        public static Opcode LXI_B = new Opcode(OpcodeBytes.LXI_B, 3, "LXI B,d16", 10); //B <- byte 3, C <- byte 2

        public static Dictionary<byte, Opcode> Lookup = new Dictionary<byte, Opcode>()
        {
            [OpcodeBytes.NOP] = NOP,
            [OpcodeBytes.LXI_B] = LXI_B,
        };
    }
}
