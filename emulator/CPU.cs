using System;
using System.Collections.Generic;

namespace JustinCredible.SIEmulator
{
    public class CPU
    {
        /**
         * Indicates the ROM has finished executing via a TODO opcode.
         * Step should not be called again without first calling Reset.
         */
        public bool Finished { get; private set; }

        /**
         * ROM (8K)
         * $0000-$07ff:  invaders.h
         * $0800-$0fff:  invaders.g
         * $1000-$17ff:  invaders.f
         * $1800-$1fff:  invaders.e
         * 
         * RAM (8K)
         * $2000-$23ff:  work RAM (1K)
         * $2400-$3fff:  video RAM (7K)
         * 
         * $4000-:       RAM mirror
         */
        private byte[] _memory;

        /** The primary CPU registers. */
        private Registers _registers;

        /** The encapsulated condition/flags regiser. */
        private ConditionFlags _flags;

        /** Program Counter; 16-bits */
        private UInt16 _programCounter;

        /** Stack Pointer; 16-bits */
        private UInt16 _stackPointer;

        public CPU()
        {
            this.Reset();
        }

        public void Reset(int seed = -1)
        {
            // Initialize the regisgters and memory.
            _memory = new byte[16*1024];
            _registers = new Registers();
            _flags = new ConditionFlags();

            // The ROMs are loaded at the lower 8K of addressable memory.
            _programCounter = 0x0000;

            // Initialize the stack pointer.
            _stackPointer = 0x0000; // TODO: ???

            // Reset the flag that indicates that the ROM has finished executing.
            Finished = false;
        }

        private void LoadMemory(byte[] memory)
        {
            _memory = memory;
        }

        public void LoadRom(byte[] rom)
        {
            // Ensure the ROM data is not larger than we can load.
            if (rom.Length > 8192)
                throw new Exception("ROM filesize cannot exceed 8 kilobytes.");

            var memory = new byte[16*1024];

            // The ROM is the lower 8K of addressable memory.
            Array.Copy(rom, memory, 8192);

            LoadMemory(memory);
        }

        public CPUState DumpState()
        {
            return new CPUState()
            {
                Memory = _memory,
                Registers = _registers,
                Flags = _flags,
                ProgramCounter = _programCounter,
                StackPointer = _stackPointer,
            };
        }

        public void PrintDebugSummary()
        {
            var opcodeByte = _memory[_programCounter];
            var opcodeInstruction = OpcodeTable.Lookup[opcodeByte].Instruction;

            var opcode = String.Format("0x{0:X2} {1}", opcodeByte, opcodeInstruction);
            var pc = String.Format("0x{0:X4}", _programCounter);
            var sp = String.Format("0x{0:X4}", _stackPointer);
            var regA = String.Format("0x{0:X2}", _registers.A);
            var regB = String.Format("0x{0:X2}", _registers.B);
            var regC = String.Format("0x{0:X2}", _registers.C);
            var regD = String.Format("0x{0:X2}", _registers.D);
            var regE = String.Format("0x{0:X2}", _registers.E);
            var regH = String.Format("0x{0:X2}", _registers.H);
            var regL = String.Format("0x{0:X2}", _registers.L);

            Console.WriteLine($"Opcode: ${opcode}");
            Console.WriteLine($"PC: ${pc}\tSP: ${sp}");
            Console.WriteLine($"A: ${regA}\tB: ${regB}\tC: ${regC}\tD: ${regD}");
            Console.WriteLine($"E: ${regE}\tH: ${regH}\tL: ${regL}");
            Console.WriteLine($"Zero: ${_flags.Zero}\tSign: ${_flags.Sign}\tParity: ${_flags.Parity}");
            Console.WriteLine($"Carry: ${_flags.Carry}\tAuxillary Carry: ${_flags.AuxCarry}");
        }

        /**
         * Executes the next instruction and returns the number of cycles it took to execute.
         */
        public int Step()
        {
            // Sanity check.
            if (Finished)
                throw new Exception("Program has finished execution; Reset() must be invoked before invoking Step() again.");

            // Fetch the next opcode to be executed.
            var opcodeByte = _memory[_programCounter];
            var opcode = OpcodeTable.Lookup[opcodeByte];

            // Some instructions have an alternate cycle count depending on the outcome of
            // the operation. This indicates how we should increment the program counter.
            var useAlternateCycleCount = false;

            // Execute the opcode.
            switch (opcodeByte)
            {
                case OpcodeBytes.NOP:
                    break;

                case OpcodeBytes.LXI_B:
                    _registers.B = _memory[_programCounter + 2];
                    _registers.C = _memory[_programCounter + 1];
                    break;

                default:
                    throw new NotImplementedException(String.Format("Attempted to execute unknown opcode 0x{0:X2} at memory address 0x{0:X4}", opcode, _programCounter));
            }

            // Determine how many cycles the instruction took.

            var elapsedCycles = (UInt16)opcode.Cycles;

            if (useAlternateCycleCount)
            {
                // Sanity check; if this fails an opcode definition or implementation is invalid.
                if (opcode.AlternateCycles == null)
                    throw new Exception(String.Format("The implementation for opcode 0x{0:X2} at memory address 0x{0:X4} indicated the alternate number of cycles should be used, but was not defined.", opcode, _programCounter));

                elapsedCycles = (UInt16)opcode.AlternateCycles;
            }

            // Increment the program counter.
            _programCounter += elapsedCycles;

            return elapsedCycles;
        }
    }
}
