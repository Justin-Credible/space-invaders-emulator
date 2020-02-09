
namespace JustinCredible.Intel8080
{
    /**
     * Describes an opcode for the Intel 8080 CPU.
     */
    public class Opcode
    {
        public Opcode(byte code, int size, string instruction, int cycles, int? alternateCycles = null, string pseudocode = null)
        {
            Code = code;
            Size = size;
            Instruction = instruction;
            Cycles = cycles;
            AlternateCycles = alternateCycles;
            Pseudocode = pseudocode;
        }

        public byte Code { get; set; }
        public int Size { get; set; }
        public string Instruction { get; set; }
        public int Cycles { get; set; }
        public int? AlternateCycles { get; set; }
        public string Pseudocode { get; set; }
    }
}
