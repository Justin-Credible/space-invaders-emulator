using System;
using System.Collections.Generic;

namespace JustinCredible.SIEmulator.Tests
{
    public class InitialCPUState
    {
        public byte[] Memory { get; set; }
        public CPURegisters Registers { get; set; }
        public ConditionFlags Flags { get; set; }
        public UInt16? ProgramCounter { get; set; }
        public UInt16? StackPointer { get; set; }
    }
}
