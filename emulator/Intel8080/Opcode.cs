
namespace JustinCredible.SIEmulator
{
    public class Opcode
    {
        public Opcode(byte code, int size, string instruction, int cycles, int? alternateCycles = null)
        {
            Code = code;
            Size = size;
            Instruction = instruction;
            Cycles = cycles;
            AlternateCycles = alternateCycles;
        }

        public byte Code { get; set; }
        public int Size { get; set; }
        public string Instruction { get; set; }
        public int Cycles { get; set; }
        public int? AlternateCycles { get; set; }
    }
}
