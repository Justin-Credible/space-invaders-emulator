using System;
using System.Collections.Generic;

namespace JustinCredible.SIEmulator.Tests
{
    public class UnitTestCPUState : CPUState
    {
        /** The number of CPU steps or instructions executed */
        public int Iterations { get; set; }

        /** The number of CPU cycles ellapsed. */
        public int Cycles { get; set; }

        /** The values of the program counter at each CPU step that occurred. */
        public List<UInt16> ProgramCounterAddresses { get; set; }

        public UnitTestCPUState(CPUState state)
        {
            this.Memory = state.Memory;
            this.Registers = state.Registers;
            this.Flags = state.Flags;
            this.ProgramCounter = state.ProgramCounter;
            this.StackPointer = state.StackPointer;
        }
    }
}
