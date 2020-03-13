using System;
using System.Collections.Generic;

namespace JustinCredible.Intel8080
{
    /**
     * Initial configuration for the CPU; values also used when resetting the CPU.
     */
    public class CPUConfig
    {
        /** Size, in bytes, of the memory space for the CPU. */
        public int MemorySize { get; set; }

        /** Starting memory location that should be writeable (leave both to 0 to allow all writes). */
        public int WriteableMemoryStart { get; set; }

        /** Ending memory location that should be writeable (leave both to 0 to allow all writes). */
        public int WriteableMemoryEnd { get; set; }

        /** Starting memory location that should be mirrored to the writeable memory (RAM mirror)  (leave bot to 0 to disable RAM mirror). */
        public int MirrorMemoryStart { get; set; }

        /** Ending memory location that should be mirrored to the writeable memory (RAM mirror) (leave bot to 0 to disable RAM mirror). */
        public int MirrorMemoryEnd { get; set; }

        public CPURegisters Registers { get; set; }

        public ConditionFlags Flags { get; set; }

        public UInt16 ProgramCounter { get; set; } = 0x0000;

        public UInt16 StackPointer { get; set; } = 0x0000;

        public bool InterruptsEnabled { get; set; } = false;

        /**
         * Special flag used to patch the CALL calls for the cpudiag.bin program.
         * This allow CALL 0x05 to simulate CP/M writing the console and will exit
         * on JMP 0x00. This is only used for testing the CPU with this specific ROM.
         */
        public bool EnableDiagnosticsMode { get; set; } = false;
    }
}
