using System;

namespace JustinCredible.SIEmulator
{
    public class CPUState
    {
        public byte[] Memory { get; set; }
        public Registers Registers { get; set; }
        public UInt16 ProgramCounter { get; set; }
        public UInt16 StackPointer { get; set; }
    }
}
