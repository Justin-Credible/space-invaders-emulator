using System;
using JustinCredible.Intel8080;

namespace JustinCredible.SIEmulator
{
    public class EmulatorState
    {
        public CPURegisters Registers { get; set; }
        public ConditionFlags Flags { get; set; }
        public UInt16 ProgramCounter { get; set; }
        public UInt16 StackPointer { get; set; }
        public bool InterruptsEnabled { get; set; }
        public byte[] Memory { get; set; }
        public int TotalCycles { get; set; }
        public int TotalSteps { get; set; }

        public int? LastCyclesExecuted { get; set; }
    }
}
