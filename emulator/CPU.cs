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
        public byte[] Memory { get; set; }

        public CPURegisters _registers;

        /** The primary CPU registers. */
        public CPURegisters Registers
        {
            get
            {
                return _registers;
            }

            set
            {
                _registers = value;
            }
        }

        /** The encapsulated condition/flags regiser. */
        public ConditionFlags Flags { get; set; }

        /** Program Counter; 16-bits */
        public UInt16 ProgramCounter { get; set; }

        /** Stack Pointer; 16-bits */
        public UInt16 StackPointer { get; set; }

        public CPU()
        {
            this.Reset();
        }

        public void Reset()
        {
            // Initialize the regisgters and memory.
            Memory = new byte[16*1024];
            _registers = new CPURegisters();
            Flags = new ConditionFlags();

            // The ROMs are loaded at the lower 8K of addressable memory.
            ProgramCounter = 0x0000;

            // Initialize the stack pointer.
            StackPointer = 0x0000; // TODO: ???

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

            Memory = memory;
        }

        public void PrintDebugSummary()
        {
            var opcodeByte = Memory[ProgramCounter];
            var opcodeInstruction = OpcodeTable.Lookup[opcodeByte].Instruction;

            var opcode = String.Format("0x{0:X2} {1}", opcodeByte, opcodeInstruction);
            var pc = String.Format("0x{0:X4}", ProgramCounter);
            var sp = String.Format("0x{0:X4}", StackPointer);
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
            Console.WriteLine($"Zero: ${Flags.Zero}\tSign: ${Flags.Sign}\tParity: ${Flags.Parity}");
            Console.WriteLine($"Carry: ${Flags.Carry}\tAuxillary Carry: ${Flags.AuxCarry}");
        }

        /** Executes the next instruction and returns the number of cycles it took to execute. */
        public int Step()
        {
            // Sanity check.
            if (Finished)
                throw new Exception("Program has finished execution; Reset() must be invoked before invoking Step() again.");

            // Fetch the next opcode to be executed.
            var opcodeByte = Memory[ProgramCounter];
            var opcode = OpcodeTable.Lookup[opcodeByte];

            // Some instructions have an alternate cycle count depending on the outcome of
            // the operation. This indicates how we should increment the program counter.
            var useAlternateCycleCount = false;

            // Execute the opcode.
            switch (opcodeByte)
            {
                case OpcodeBytes.NOP:
                case OpcodeBytes.NOP2:
                case OpcodeBytes.NOP3:
                case OpcodeBytes.NOP4:
                case OpcodeBytes.NOP5:
                case OpcodeBytes.NOP6:
                case OpcodeBytes.NOP7:
                case OpcodeBytes.NOP8:
                    break;

                case OpcodeBytes.HLT:
                    Finished = true;
                    break;

                case OpcodeBytes.STA:
                {
                    var upper = Memory[ProgramCounter + 2] << 8;
                    var lower = Memory[ProgramCounter + 1];
                    var address = upper | lower;
                    Memory[address] = _registers.A;
                    break;
                }

                case OpcodeBytes.LDA:
                {
                    var upper = Memory[ProgramCounter + 2] << 8;
                    var lower = Memory[ProgramCounter + 1];
                    var address = upper | lower;
                    _registers.A = Memory[address];
                    break;
                }

                #region MOV

                #region MOV X, X (from register to register)

                case OpcodeBytes.MOV_B_B:
                    // NOP
                    break;
                case OpcodeBytes.MOV_B_C:
                    _registers.B = _registers.C;
                    break;
                case OpcodeBytes.MOV_B_D:
                    _registers.B = _registers.D;
                    break;
                case OpcodeBytes.MOV_B_E:
                    _registers.B = _registers.E;
                    break;
                case OpcodeBytes.MOV_B_H:
                    _registers.B = _registers.H;
                    break;
                case OpcodeBytes.MOV_B_L:
                    _registers.B = _registers.L;
                    break;
                case OpcodeBytes.MOV_B_A:
                    _registers.B = _registers.A;
                    break;
                case OpcodeBytes.MOV_C_B:
                    _registers.C = _registers.B;
                    break;
                case OpcodeBytes.MOV_C_C:
                    // NOP
                    break;
                case OpcodeBytes.MOV_C_D:
                    _registers.C = _registers.D;
                    break;
                case OpcodeBytes.MOV_C_E:
                    _registers.C = _registers.E;
                    break;
                case OpcodeBytes.MOV_C_H:
                    _registers.C = _registers.H;
                    break;
                case OpcodeBytes.MOV_C_L:
                    _registers.C = _registers.L;
                    break;
                case OpcodeBytes.MOV_C_A:
                    _registers.C = _registers.A;
                    break;
                case OpcodeBytes.MOV_D_B:
                    _registers.D = _registers.B;
                    break;
                case OpcodeBytes.MOV_D_C:
                    _registers.D = _registers.C;
                    break;
                case OpcodeBytes.MOV_D_D:
                    // NOP
                    break;
                case OpcodeBytes.MOV_D_E:
                    _registers.D = _registers.E;
                    break;
                case OpcodeBytes.MOV_D_H:
                    _registers.D = _registers.H;
                    break;
                case OpcodeBytes.MOV_D_L:
                    _registers.D = _registers.L;
                    break;
                case OpcodeBytes.MOV_D_A:
                    _registers.D = _registers.A;
                    break;
                case OpcodeBytes.MOV_E_B:
                    _registers.E = _registers.B;
                    break;
                case OpcodeBytes.MOV_E_C:
                    _registers.E = _registers.C;
                    break;
                case OpcodeBytes.MOV_E_D:
                    _registers.E = _registers.D;
                    break;
                case OpcodeBytes.MOV_E_E:
                    // NOP
                    break;
                case OpcodeBytes.MOV_E_H:
                    _registers.E = _registers.H;
                    break;
                case OpcodeBytes.MOV_E_L:
                    _registers.E = _registers.L;
                    break;
                case OpcodeBytes.MOV_E_A:
                    _registers.E = _registers.A;
                    break;
                case OpcodeBytes.MOV_H_B:
                    _registers.H = _registers.B;
                    break;
                case OpcodeBytes.MOV_H_C:
                    _registers.H = _registers.C;
                    break;
                case OpcodeBytes.MOV_H_D:
                    _registers.H = _registers.D;
                    break;
                case OpcodeBytes.MOV_H_E:
                    _registers.H = _registers.E;
                    break;
                case OpcodeBytes.MOV_H_H:
                    // NOP
                    break;
                case OpcodeBytes.MOV_H_L:
                    _registers.H = _registers.L;
                    break;
                case OpcodeBytes.MOV_H_A:
                    _registers.H = _registers.A;
                    break;
                case OpcodeBytes.MOV_L_B:
                    _registers.L = _registers.B;
                    break;
                case OpcodeBytes.MOV_L_C:
                    _registers.L = _registers.C;
                    break;
                case OpcodeBytes.MOV_L_D:
                    _registers.L = _registers.D;
                    break;
                case OpcodeBytes.MOV_L_E:
                    _registers.L = _registers.E;
                    break;
                case OpcodeBytes.MOV_L_H:
                    _registers.L = _registers.H;
                    break;
                case OpcodeBytes.MOV_L_L:
                    // NOP
                    break;
                case OpcodeBytes.MOV_L_A:
                    _registers.L = _registers.A;
                    break;
                case OpcodeBytes.MOV_A_B:
                    _registers.A = _registers.B;
                    break;
                case OpcodeBytes.MOV_A_C:
                    _registers.A = _registers.C;
                    break;
                case OpcodeBytes.MOV_A_D:
                    _registers.A = _registers.D;
                    break;
                case OpcodeBytes.MOV_A_E:
                    _registers.A = _registers.E;
                    break;
                case OpcodeBytes.MOV_A_H:
                    _registers.A = _registers.H;
                    break;
                case OpcodeBytes.MOV_A_L:
                    _registers.A = _registers.L;
                    break;
                case OpcodeBytes.MOV_A_A:
                    // NOP
                    break;

                #endregion

                #region MOV X, M (from memory to register)

                case OpcodeBytes.MOV_B_M:
                    _registers.B = Memory[GetAddress()];
                    break;
                case OpcodeBytes.MOV_C_M:
                    _registers.C = Memory[GetAddress()];
                    break;
                case OpcodeBytes.MOV_D_M:
                    _registers.D = Memory[GetAddress()];
                    break;
                case OpcodeBytes.MOV_E_M:
                    _registers.E = Memory[GetAddress()];
                    break;
                case OpcodeBytes.MOV_H_M:
                    _registers.H = Memory[GetAddress()];
                    break;
                case OpcodeBytes.MOV_L_M:
                    _registers.L = Memory[GetAddress()];
                    break;
                case OpcodeBytes.MOV_A_M:
                    _registers.A = Memory[GetAddress()];
                    break;

                #endregion

                #region MOV M, X (from register to memory)

                case OpcodeBytes.MOV_M_B:
                    ExecuteMOVFromRegisterToMemory(Register.B);
                    break;
                case OpcodeBytes.MOV_M_C:
                    ExecuteMOVFromRegisterToMemory(Register.C);
                    break;
                case OpcodeBytes.MOV_M_D:
                    ExecuteMOVFromRegisterToMemory(Register.D);
                    break;
                case OpcodeBytes.MOV_M_E:
                    ExecuteMOVFromRegisterToMemory(Register.E);
                    break;
                case OpcodeBytes.MOV_M_H:
                    ExecuteMOVFromRegisterToMemory(Register.H);
                    break;
                case OpcodeBytes.MOV_M_L:
                    ExecuteMOVFromRegisterToMemory(Register.L);
                    break;
                case OpcodeBytes.MOV_M_A:
                    ExecuteMOVFromRegisterToMemory(Register.A);
                    break;

                #endregion

                #endregion

                #region MVI

                case OpcodeBytes.MVI_B:
                    _registers.B = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_C:
                    _registers.C = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_D:
                    _registers.D = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_E:
                    _registers.E = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_H:
                    _registers.H = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_L:
                    _registers.L = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_M:
                    ExecuteMOVIToMemory(Memory[ProgramCounter + 1]);
                    break;
                case OpcodeBytes.MVI_A:
                    _registers.A = Memory[ProgramCounter + 1];
                    break;

                #endregion

                #region LXI
                case OpcodeBytes.LXI_B:
                    _registers.B = Memory[ProgramCounter + 2];
                    _registers.C = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.LXI_D:
                    _registers.D = Memory[ProgramCounter + 2];
                    _registers.E = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.LXI_H:
                    _registers.H = Memory[ProgramCounter + 2];
                    _registers.L = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.LXI_SP:
                {
                    var upper = Memory[ProgramCounter + 2] << 8;
                    var lower = Memory[ProgramCounter + 1];
                    var address = upper | lower;
                    StackPointer = (UInt16)address;
                    break;
                }
                #endregion

                #region STAX
                case OpcodeBytes.STAX_B:
                {
                    var address = GetAddress(Register.B, Register.C);
                    Memory[address] = _registers.A;
                    break;
                }
                case OpcodeBytes.STAX_D:
                {
                    var address = GetAddress(Register.D, Register.E);
                    Memory[address] = _registers.A;
                    break;
                }
                #endregion

                #region LDAX
                case OpcodeBytes.LDAX_B:
                {
                    var address = GetAddress(Register.B, Register.C);
                    _registers.A = Memory[address];
                    break;
                }
                case OpcodeBytes.LDAX_D:
                {
                    var address = GetAddress(Register.D, Register.E);
                    _registers.A = Memory[address];
                    break;
                }
                #endregion

                #region INX
                case OpcodeBytes.INX_B:
                {
                    var upper = _registers.B << 8;
                    var lower = _registers.C;
                    var value = upper | lower;
                    value++;
                    _registers.B = (byte)((0xFF00 & value) >> 8);
                    _registers.C = (byte)(0x00FF & value);
                    break;
                }
                case OpcodeBytes.INX_D:
                {
                    var upper = _registers.D << 8;
                    var lower = _registers.E;
                    var value = upper | lower;
                    value++;
                    _registers.D = (byte)((0xFF00 & value) >> 8);
                    _registers.E = (byte)(0x00FF & value);
                    break;
                }
                case OpcodeBytes.INX_H:
                {
                    var upper = _registers.H << 8;
                    var lower = _registers.L;
                    var value = upper | lower;
                    value++;
                    _registers.H = (byte)((0xFF00 & value) >> 8);
                    _registers.L = (byte)(0x00FF & value);
                    break;
                }
                case OpcodeBytes.INX_SP:
                    StackPointer++;
                    break;
                #endregion

                default:
                    throw new NotImplementedException(String.Format("Attempted to execute unknown opcode 0x{0:X2} at memory address 0x{0:X4}", opcode, ProgramCounter));
            }

            // Determine how many cycles the instruction took.

            var elapsedCycles = (UInt16)opcode.Cycles;

            if (useAlternateCycleCount)
            {
                // Sanity check; if this fails an opcode definition or implementation is invalid.
                if (opcode.AlternateCycles == null)
                    throw new Exception(String.Format("The implementation for opcode 0x{0:X2} at memory address 0x{0:X4} indicated the alternate number of cycles should be used, but was not defined.", opcode, ProgramCounter));

                elapsedCycles = (UInt16)opcode.AlternateCycles;
            }

            // Increment the program counter.
            if (opcode != OpcodeTable.HLT)
               ProgramCounter += (UInt16)opcode.Size;

            return elapsedCycles;
        }

        /**
         * Used to build a memory address from the two given register values.
         * Using registers H and L are the most common for this, so they are the default parameters.
         */
        private UInt16 GetAddress(Register upperReg = Register.H, Register lowerReg = Register.L)
        {
            var upper = _registers[upperReg] << 8;
            var lower = _registers[lowerReg];
            var address = upper | lower;
            return (UInt16)address;
        }

        private void ExecuteMOV(Register dest, Register source)
        {
            _registers[dest] = _registers[source];
        }

        private void ExecuteMOVFromRegisterToMemory(Register source)
        {
            var address = GetAddress();

            // TODO: Should allow write to memory mirror area?
            // TODO: Should not panic on ROM area write?

            // Only allow writes to the work/video RAM and not the ROM or memory mirror.
            // $2000-$23ff:  work RAM (1K)
            // $2400-$3fff:  video RAM (7K)
            if (address >= 0x2000 && address <= 0x3FFF)
                Memory[address] = _registers[source];
            else
            {
                var addressFormatted = String.Format("0x{0:X4}", address);
                throw new Exception($"Illegal memory address ({addressFormatted}) specified for 'MOV M, {source}' operation; expected address to be between 0x2000 and 0x3FFF inclusive.");
            }
        }

        private void ExecuteMOVIToMemory(byte data)
        {
            var address = GetAddress();

            // TODO: Should allow write to memory mirror area?
            // TODO: Should not panic on ROM area write?

            // Only allow writes to the work/video RAM and not the ROM or memory mirror.
            // $2000-$23ff:  work RAM (1K)
            // $2400-$3fff:  video RAM (7K)
            if (address >= 0x2000 && address <= 0x3FFF)
                Memory[address] = data;
            else
            {
                var addressFormatted = String.Format("0x{0:X4}", address);
                throw new Exception($"Illegal memory address ({addressFormatted}) specified for 'MVI M, d8' operation; expected address to be between 0x2000 and 0x3FFF inclusive.");
            }
        }
    }
}
