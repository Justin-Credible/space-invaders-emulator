using System;

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

        public void Reset()
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

        public void LoadRom(byte[] rom)
        {
            // Ensure the ROM data is not larger than we can load.
            if (rom.Length > 8192)
                throw new Exception("ROM filesize cannot exceed 8 kilobytes.");

            var memory = new byte[16384];

            // The ROM is the lower 8K of addressable memory.
            Array.Copy(rom, memory, rom.Length);

            LoadMemory(memory);
        }

        public void LoadMemory(byte[] memory)
        {
            // Ensure the memory data is not larger than we can load.
            if (memory.Length > 16384)
                throw new Exception("Memory cannot exceed 16 kilobytes.");

            _memory = memory;
        }

        public void LoadRegisters(Registers registers)
        {
            _registers = registers;
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

        /** Executes the next instruction and returns the number of cycles it took to execute. */
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

                case OpcodeBytes.HLT:
                    Finished = true;
                    break;

                case OpcodeBytes.LXI_B:
                    _registers.B = _memory[_programCounter + 2];
                    _registers.C = _memory[_programCounter + 1];
                    break;

                #region MOV

                #region MOV X, X (from register to register)

                case OpcodeBytes.MOV_B_B:
                     _registers[RegisterID.B] = _registers[RegisterID.B];
                     break;
                case OpcodeBytes.MOV_B_C:
                     _registers[RegisterID.B] = _registers[RegisterID.C];
                     break;
                case OpcodeBytes.MOV_B_D:
                     _registers[RegisterID.B] = _registers[RegisterID.D];
                     break;
                case OpcodeBytes.MOV_B_E:
                     _registers[RegisterID.B] = _registers[RegisterID.E];
                     break;
                case OpcodeBytes.MOV_B_H:
                     _registers[RegisterID.B] = _registers[RegisterID.H];
                     break;
                case OpcodeBytes.MOV_B_L:
                     _registers[RegisterID.B] = _registers[RegisterID.L];
                     break;
                case OpcodeBytes.MOV_B_A:
                     _registers[RegisterID.B] = _registers[RegisterID.A];
                     break;
                case OpcodeBytes.MOV_C_B:
                     _registers[RegisterID.C] = _registers[RegisterID.B];
                     break;
                case OpcodeBytes.MOV_C_C:
                     _registers[RegisterID.C] = _registers[RegisterID.C];
                     break;
                case OpcodeBytes.MOV_C_D:
                     _registers[RegisterID.C] = _registers[RegisterID.D];
                     break;
                case OpcodeBytes.MOV_C_E:
                     _registers[RegisterID.C] = _registers[RegisterID.E];
                     break;
                case OpcodeBytes.MOV_C_H:
                     _registers[RegisterID.C] = _registers[RegisterID.H];
                     break;
                case OpcodeBytes.MOV_C_L:
                     _registers[RegisterID.C] = _registers[RegisterID.L];
                     break;
                case OpcodeBytes.MOV_C_A:
                     _registers[RegisterID.C] = _registers[RegisterID.A];
                     break;
                case OpcodeBytes.MOV_D_B:
                     _registers[RegisterID.D] = _registers[RegisterID.B];
                     break;
                case OpcodeBytes.MOV_D_C:
                     _registers[RegisterID.D] = _registers[RegisterID.C];
                     break;
                case OpcodeBytes.MOV_D_D:
                     _registers[RegisterID.D] = _registers[RegisterID.D];
                     break;
                case OpcodeBytes.MOV_D_E:
                     _registers[RegisterID.D] = _registers[RegisterID.E];
                     break;
                case OpcodeBytes.MOV_D_H:
                     _registers[RegisterID.D] = _registers[RegisterID.H];
                     break;
                case OpcodeBytes.MOV_D_L:
                     _registers[RegisterID.D] = _registers[RegisterID.L];
                     break;
                case OpcodeBytes.MOV_D_A:
                     _registers[RegisterID.D] = _registers[RegisterID.A];
                     break;
                case OpcodeBytes.MOV_E_B:
                     _registers[RegisterID.E] = _registers[RegisterID.B];
                     break;
                case OpcodeBytes.MOV_E_C:
                     _registers[RegisterID.E] = _registers[RegisterID.C];
                     break;
                case OpcodeBytes.MOV_E_D:
                     _registers[RegisterID.E] = _registers[RegisterID.D];
                     break;
                case OpcodeBytes.MOV_E_E:
                     _registers[RegisterID.E] = _registers[RegisterID.E];
                     break;
                case OpcodeBytes.MOV_E_H:
                     _registers[RegisterID.E] = _registers[RegisterID.H];
                     break;
                case OpcodeBytes.MOV_E_L:
                     _registers[RegisterID.E] = _registers[RegisterID.L];
                     break;
                case OpcodeBytes.MOV_E_A:
                     _registers[RegisterID.E] = _registers[RegisterID.A];
                     break;
                case OpcodeBytes.MOV_H_B:
                     _registers[RegisterID.H] = _registers[RegisterID.B];
                     break;
                case OpcodeBytes.MOV_H_C:
                     _registers[RegisterID.H] = _registers[RegisterID.C];
                     break;
                case OpcodeBytes.MOV_H_D:
                     _registers[RegisterID.H] = _registers[RegisterID.D];
                     break;
                case OpcodeBytes.MOV_H_E:
                     _registers[RegisterID.H] = _registers[RegisterID.E];
                     break;
                case OpcodeBytes.MOV_H_H:
                     _registers[RegisterID.H] = _registers[RegisterID.H];
                     break;
                case OpcodeBytes.MOV_H_L:
                     _registers[RegisterID.H] = _registers[RegisterID.L];
                     break;
                case OpcodeBytes.MOV_H_A:
                     _registers[RegisterID.H] = _registers[RegisterID.A];
                     break;
                case OpcodeBytes.MOV_L_B:
                     _registers[RegisterID.L] = _registers[RegisterID.B];
                     break;
                case OpcodeBytes.MOV_L_C:
                     _registers[RegisterID.L] = _registers[RegisterID.C];
                     break;
                case OpcodeBytes.MOV_L_D:
                     _registers[RegisterID.L] = _registers[RegisterID.D];
                     break;
                case OpcodeBytes.MOV_L_E:
                     _registers[RegisterID.L] = _registers[RegisterID.E];
                     break;
                case OpcodeBytes.MOV_L_H:
                     _registers[RegisterID.L] = _registers[RegisterID.H];
                     break;
                case OpcodeBytes.MOV_L_L:
                     _registers[RegisterID.L] = _registers[RegisterID.L];
                     break;
                case OpcodeBytes.MOV_L_A:
                     _registers[RegisterID.L] = _registers[RegisterID.A];
                     break;
                case OpcodeBytes.MOV_A_B:
                     _registers[RegisterID.A] = _registers[RegisterID.B];
                     break;
                case OpcodeBytes.MOV_A_C:
                     _registers[RegisterID.A] = _registers[RegisterID.C];
                     break;
                case OpcodeBytes.MOV_A_D:
                     _registers[RegisterID.A] = _registers[RegisterID.D];
                     break;
                case OpcodeBytes.MOV_A_E:
                     _registers[RegisterID.A] = _registers[RegisterID.E];
                     break;
                case OpcodeBytes.MOV_A_H:
                     _registers[RegisterID.A] = _registers[RegisterID.H];
                     break;
                case OpcodeBytes.MOV_A_L:
                     _registers[RegisterID.A] = _registers[RegisterID.L];
                     break;
                case OpcodeBytes.MOV_A_A:
                     _registers[RegisterID.A] = _registers[RegisterID.A];
                     break;

                #endregion

                #region MOV X, M (from memory to register)

                case OpcodeBytes.MOV_B_M:
                    ExecuteMOVFromMemoryToRegister(RegisterID.B);
                    break;
                case OpcodeBytes.MOV_C_M:
                    ExecuteMOVFromMemoryToRegister(RegisterID.C);
                    break;
                case OpcodeBytes.MOV_D_M:
                    ExecuteMOVFromMemoryToRegister(RegisterID.D);
                    break;
                case OpcodeBytes.MOV_E_M:
                    ExecuteMOVFromMemoryToRegister(RegisterID.E);
                    break;
                case OpcodeBytes.MOV_H_M:
                    ExecuteMOVFromMemoryToRegister(RegisterID.H);
                    break;
                case OpcodeBytes.MOV_L_M:
                    ExecuteMOVFromMemoryToRegister(RegisterID.L);
                    break;
                case OpcodeBytes.MOV_A_M:
                    ExecuteMOVFromMemoryToRegister(RegisterID.A);
                    break;

                #endregion

                #region MOV M, X (from register to memory)

                case OpcodeBytes.MOV_M_B:
                    ExecuteMOVFromRegisterToMemory(RegisterID.B);
                    break;
                case OpcodeBytes.MOV_M_C:
                    ExecuteMOVFromRegisterToMemory(RegisterID.C);
                    break;
                case OpcodeBytes.MOV_M_D:
                    ExecuteMOVFromRegisterToMemory(RegisterID.D);
                    break;
                case OpcodeBytes.MOV_M_E:
                    ExecuteMOVFromRegisterToMemory(RegisterID.E);
                    break;
                case OpcodeBytes.MOV_M_H:
                    ExecuteMOVFromRegisterToMemory(RegisterID.H);
                    break;
                case OpcodeBytes.MOV_M_L:
                    ExecuteMOVFromRegisterToMemory(RegisterID.L);
                    break;
                case OpcodeBytes.MOV_M_A:
                    ExecuteMOVFromRegisterToMemory(RegisterID.A);
                    break;

                #endregion

                #endregion

                #region MVI

                case OpcodeBytes.MVI_B:
                    _registers[RegisterID.B] = _memory[_programCounter + 1];
                    break;
                case OpcodeBytes.MVI_C:
                    _registers[RegisterID.C] = _memory[_programCounter + 1];
                    break;
                case OpcodeBytes.MVI_D:
                    _registers[RegisterID.D] = _memory[_programCounter + 1];
                    break;
                case OpcodeBytes.MVI_E:
                    _registers[RegisterID.E] = _memory[_programCounter + 1];
                    break;
                case OpcodeBytes.MVI_H:
                    _registers[RegisterID.H] = _memory[_programCounter + 1];
                    break;
                case OpcodeBytes.MVI_L:
                    _registers[RegisterID.L] = _memory[_programCounter + 1];
                    break;
                case OpcodeBytes.MVI_M:
                    ExecuteMOVIToMemory(_memory[_programCounter + 1]);
                    break;
                case OpcodeBytes.MVI_A:
                    _registers[RegisterID.A] = _memory[_programCounter + 1];
                    break;

                #endregion

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
            if (opcode != OpcodeTable.HLT)
               _programCounter += (UInt16)opcode.Size;

            return elapsedCycles;
        }

        private void ExecuteMOV(RegisterID dest, RegisterID source)
        {
            _registers[dest] = _registers[source];
        }

        private void ExecuteMOVFromMemoryToRegister(RegisterID dest)
        {
            var upper = _registers.H << 8;
            var lower = (_registers.L);
            var address = upper | lower;

            _registers[dest] = _memory[address];
        }

        private void ExecuteMOVFromRegisterToMemory(RegisterID source)
        {
            var upper = _registers.H << 8;
            var lower = (_registers.L);
            var address = upper | lower;

            // TODO: Should allow write to memory mirror area?
            // TODO: Should not panic on ROM area write?

            // Only allow writes to the work/video RAM and not the ROM or memory mirror.
            // $2000-$23ff:  work RAM (1K)
            // $2400-$3fff:  video RAM (7K)
            if (address >= 0x2000 && address <= 0x3FFFF)
                _memory[address] = _registers[source];
            else
            {
                var addressFormatted = String.Format("0x{0:X4}", address);
                throw new Exception($"Illegal memory address ({addressFormatted}) specified for 'MOV M, {source}' operation; expected address to be between 0x2000 and 0x3FFF inclusive.");
            }
        }

        private void ExecuteMOVIToMemory(byte data)
        {
            var upper = _registers.H << 8;
            var lower = (_registers.L);
            var address = upper | lower;

            // TODO: Should allow write to memory mirror area?
            // TODO: Should not panic on ROM area write?

            // Only allow writes to the work/video RAM and not the ROM or memory mirror.
            // $2000-$23ff:  work RAM (1K)
            // $2400-$3fff:  video RAM (7K)
            if (address >= 0x2000 && address <= 0x3FFFF)
                _memory[address] = data;
            else
            {
                var addressFormatted = String.Format("0x{0:X4}", address);
                throw new Exception($"Illegal memory address ({addressFormatted}) specified for 'MVI M, d8' operation; expected address to be between 0x2000 and 0x3FFF inclusive.");
            }
        }
    }
}
