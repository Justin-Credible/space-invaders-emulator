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

        /** The primary CPU registers. */
        public CPURegisters Registers { get; set; }

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
            Registers = new CPURegisters();
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
            var regA = String.Format("0x{0:X2}", Registers.A);
            var regB = String.Format("0x{0:X2}", Registers.B);
            var regC = String.Format("0x{0:X2}", Registers.C);
            var regD = String.Format("0x{0:X2}", Registers.D);
            var regE = String.Format("0x{0:X2}", Registers.E);
            var regH = String.Format("0x{0:X2}", Registers.H);
            var regL = String.Format("0x{0:X2}", Registers.L);

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
                    Memory[address] = Registers.A;
                    break;
                }

                case OpcodeBytes.LDA:
                {
                    var upper = Memory[ProgramCounter + 2] << 8;
                    var lower = Memory[ProgramCounter + 1];
                    var address = upper | lower;
                    Registers.A = Memory[address];
                    break;
                }

                case OpcodeBytes.CMA:
                    Registers.A = (byte)~Registers.A;
                    break;

                case OpcodeBytes.STC:
                    Flags.Carry = true;
                    break;

                case OpcodeBytes.CMC:
                    Flags.Carry = !Flags.Carry;
                    break;

                case OpcodeBytes.SHLD:
                {
                    var upper = Memory[ProgramCounter + 2] << 8;
                    var lower = Memory[ProgramCounter + 1];
                    var address = upper | lower;
                    Memory[address] = Registers.L;
                    Memory[address + 1] = Registers.H;
                    break;
                }

                case OpcodeBytes.LHLD:
                {
                    var upper = Memory[ProgramCounter + 2] << 8;
                    var lower = Memory[ProgramCounter + 1];
                    var address = upper | lower;
                    Registers.L = Memory[address];
                    Registers.H = Memory[address + 1];
                    break;
                }

                #region MOV

                #region MOV X, X (from register to register)

                case OpcodeBytes.MOV_B_B:
                    // NOP
                    break;
                case OpcodeBytes.MOV_B_C:
                    Registers.B = Registers.C;
                    break;
                case OpcodeBytes.MOV_B_D:
                    Registers.B = Registers.D;
                    break;
                case OpcodeBytes.MOV_B_E:
                    Registers.B = Registers.E;
                    break;
                case OpcodeBytes.MOV_B_H:
                    Registers.B = Registers.H;
                    break;
                case OpcodeBytes.MOV_B_L:
                    Registers.B = Registers.L;
                    break;
                case OpcodeBytes.MOV_B_A:
                    Registers.B = Registers.A;
                    break;
                case OpcodeBytes.MOV_C_B:
                    Registers.C = Registers.B;
                    break;
                case OpcodeBytes.MOV_C_C:
                    // NOP
                    break;
                case OpcodeBytes.MOV_C_D:
                    Registers.C = Registers.D;
                    break;
                case OpcodeBytes.MOV_C_E:
                    Registers.C = Registers.E;
                    break;
                case OpcodeBytes.MOV_C_H:
                    Registers.C = Registers.H;
                    break;
                case OpcodeBytes.MOV_C_L:
                    Registers.C = Registers.L;
                    break;
                case OpcodeBytes.MOV_C_A:
                    Registers.C = Registers.A;
                    break;
                case OpcodeBytes.MOV_D_B:
                    Registers.D = Registers.B;
                    break;
                case OpcodeBytes.MOV_D_C:
                    Registers.D = Registers.C;
                    break;
                case OpcodeBytes.MOV_D_D:
                    // NOP
                    break;
                case OpcodeBytes.MOV_D_E:
                    Registers.D = Registers.E;
                    break;
                case OpcodeBytes.MOV_D_H:
                    Registers.D = Registers.H;
                    break;
                case OpcodeBytes.MOV_D_L:
                    Registers.D = Registers.L;
                    break;
                case OpcodeBytes.MOV_D_A:
                    Registers.D = Registers.A;
                    break;
                case OpcodeBytes.MOV_E_B:
                    Registers.E = Registers.B;
                    break;
                case OpcodeBytes.MOV_E_C:
                    Registers.E = Registers.C;
                    break;
                case OpcodeBytes.MOV_E_D:
                    Registers.E = Registers.D;
                    break;
                case OpcodeBytes.MOV_E_E:
                    // NOP
                    break;
                case OpcodeBytes.MOV_E_H:
                    Registers.E = Registers.H;
                    break;
                case OpcodeBytes.MOV_E_L:
                    Registers.E = Registers.L;
                    break;
                case OpcodeBytes.MOV_E_A:
                    Registers.E = Registers.A;
                    break;
                case OpcodeBytes.MOV_H_B:
                    Registers.H = Registers.B;
                    break;
                case OpcodeBytes.MOV_H_C:
                    Registers.H = Registers.C;
                    break;
                case OpcodeBytes.MOV_H_D:
                    Registers.H = Registers.D;
                    break;
                case OpcodeBytes.MOV_H_E:
                    Registers.H = Registers.E;
                    break;
                case OpcodeBytes.MOV_H_H:
                    // NOP
                    break;
                case OpcodeBytes.MOV_H_L:
                    Registers.H = Registers.L;
                    break;
                case OpcodeBytes.MOV_H_A:
                    Registers.H = Registers.A;
                    break;
                case OpcodeBytes.MOV_L_B:
                    Registers.L = Registers.B;
                    break;
                case OpcodeBytes.MOV_L_C:
                    Registers.L = Registers.C;
                    break;
                case OpcodeBytes.MOV_L_D:
                    Registers.L = Registers.D;
                    break;
                case OpcodeBytes.MOV_L_E:
                    Registers.L = Registers.E;
                    break;
                case OpcodeBytes.MOV_L_H:
                    Registers.L = Registers.H;
                    break;
                case OpcodeBytes.MOV_L_L:
                    // NOP
                    break;
                case OpcodeBytes.MOV_L_A:
                    Registers.L = Registers.A;
                    break;
                case OpcodeBytes.MOV_A_B:
                    Registers.A = Registers.B;
                    break;
                case OpcodeBytes.MOV_A_C:
                    Registers.A = Registers.C;
                    break;
                case OpcodeBytes.MOV_A_D:
                    Registers.A = Registers.D;
                    break;
                case OpcodeBytes.MOV_A_E:
                    Registers.A = Registers.E;
                    break;
                case OpcodeBytes.MOV_A_H:
                    Registers.A = Registers.H;
                    break;
                case OpcodeBytes.MOV_A_L:
                    Registers.A = Registers.L;
                    break;
                case OpcodeBytes.MOV_A_A:
                    // NOP
                    break;

                #endregion

                #region MOV X, M (from memory to register)

                case OpcodeBytes.MOV_B_M:
                    Registers.B = Memory[Registers.HL];
                    break;
                case OpcodeBytes.MOV_C_M:
                    Registers.C = Memory[Registers.HL];
                    break;
                case OpcodeBytes.MOV_D_M:
                    Registers.D = Memory[Registers.HL];
                    break;
                case OpcodeBytes.MOV_E_M:
                    Registers.E = Memory[Registers.HL];
                    break;
                case OpcodeBytes.MOV_H_M:
                    Registers.H = Memory[Registers.HL];
                    break;
                case OpcodeBytes.MOV_L_M:
                    Registers.L = Memory[Registers.HL];
                    break;
                case OpcodeBytes.MOV_A_M:
                    Registers.A = Memory[Registers.HL];
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
                    Registers.B = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_C:
                    Registers.C = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_D:
                    Registers.D = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_E:
                    Registers.E = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_H:
                    Registers.H = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_L:
                    Registers.L = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.MVI_M:
                    ExecuteMOVIToMemory(Memory[ProgramCounter + 1]);
                    break;
                case OpcodeBytes.MVI_A:
                    Registers.A = Memory[ProgramCounter + 1];
                    break;

                #endregion

                #region LXI
                case OpcodeBytes.LXI_B:
                    Registers.B = Memory[ProgramCounter + 2];
                    Registers.C = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.LXI_D:
                    Registers.D = Memory[ProgramCounter + 2];
                    Registers.E = Memory[ProgramCounter + 1];
                    break;
                case OpcodeBytes.LXI_H:
                    Registers.H = Memory[ProgramCounter + 2];
                    Registers.L = Memory[ProgramCounter + 1];
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
                    Memory[Registers.BC] = Registers.A;
                    break;
                case OpcodeBytes.STAX_D:
                    Memory[Registers.DE] = Registers.A;
                    break;
                #endregion

                #region LDAX
                case OpcodeBytes.LDAX_B:
                    Registers.A = Memory[Registers.BC];
                    break;
                case OpcodeBytes.LDAX_D:
                    Registers.A = Memory[Registers.DE];
                    break;
                #endregion

                #region INX
                case OpcodeBytes.INX_B:
                    Registers.BC++;
                    break;
                case OpcodeBytes.INX_D:
                    Registers.DE++;
                    break;
                case OpcodeBytes.INX_H:
                    Registers.HL++;
                    break;
                case OpcodeBytes.INX_SP:
                    StackPointer++;
                    break;
                #endregion

                #region DCX
                case OpcodeBytes.DCX_B:
                    Registers.BC--;
                    break;
                case OpcodeBytes.DCX_D:
                    Registers.DE--;
                    break;
                case OpcodeBytes.DCX_H:
                    Registers.HL--;
                    break;
                case OpcodeBytes.DCX_SP:
                    StackPointer--;
                    break;
                #endregion

                #region PUSH
                case OpcodeBytes.PUSH_B:
                    Memory[StackPointer - 1] = Registers.B;
                    Memory[StackPointer - 2] = Registers.C;
                    StackPointer = (UInt16)(StackPointer - 2);
                    break;
                case OpcodeBytes.PUSH_D:
                    Memory[StackPointer - 1] = Registers.D;
                    Memory[StackPointer - 2] = Registers.E;
                    StackPointer = (UInt16)(StackPointer - 2);
                    break;
                case OpcodeBytes.PUSH_H:
                    Memory[StackPointer - 1] = Registers.H;
                    Memory[StackPointer - 2] = Registers.L;
                    StackPointer = (UInt16)(StackPointer - 2);
                    break;
                case OpcodeBytes.PUSH_PSW:
                    Memory[StackPointer - 1] = Registers.A;
                    Memory[StackPointer - 2] = Flags.ToByte();
                    StackPointer = (UInt16)(StackPointer - 2);
                    break;
                #endregion

                #region POP
                case OpcodeBytes.POP_B:
                    Registers.B = Memory[StackPointer + 1];
                    Registers.C = Memory[StackPointer];
                    StackPointer = (UInt16)(StackPointer + 2);
                    break;
                case OpcodeBytes.POP_D:
                    Registers.D = Memory[StackPointer + 1];
                    Registers.E = Memory[StackPointer];
                    StackPointer = (UInt16)(StackPointer + 2);
                    break;
                case OpcodeBytes.POP_H:
                    Registers.H = Memory[StackPointer + 1];
                    Registers.L = Memory[StackPointer];
                    StackPointer = (UInt16)(StackPointer + 2);
                    break;
                case OpcodeBytes.POP_PSW:
                    Registers.A = Memory[StackPointer + 1];
                    Flags.SetFromByte(Memory[StackPointer]);
                    StackPointer = (UInt16)(StackPointer + 2);
                    break;
                #endregion

                #region ADD
                case OpcodeBytes.ADD_B:
                    ExecuteADD(Registers.B);
                    break;
                case OpcodeBytes.ADD_C:
                    ExecuteADD(Registers.C);
                    break;
                case OpcodeBytes.ADD_D:
                    ExecuteADD(Registers.D);
                    break;
                case OpcodeBytes.ADD_E:
                    ExecuteADD(Registers.E);
                    break;
                case OpcodeBytes.ADD_H:
                    ExecuteADD(Registers.H);
                    break;
                case OpcodeBytes.ADD_L:
                    ExecuteADD(Registers.L);
                    break;
                case OpcodeBytes.ADD_M:
                    ExecuteADD(Memory[Registers.HL]);
                    break;
                case OpcodeBytes.ADD_A:
                    ExecuteADD(Registers.A);
                    break;
                #endregion

                #region SUB
                case OpcodeBytes.SUB_B:
                    ExecuteSUB(Registers.B);
                    break;
                case OpcodeBytes.SUB_C:
                    ExecuteSUB(Registers.C);
                    break;
                case OpcodeBytes.SUB_D:
                    ExecuteSUB(Registers.D);
                    break;
                case OpcodeBytes.SUB_E:
                    ExecuteSUB(Registers.E);
                    break;
                case OpcodeBytes.SUB_H:
                    ExecuteSUB(Registers.H);
                    break;
                case OpcodeBytes.SUB_L:
                    ExecuteSUB(Registers.L);
                    break;
                case OpcodeBytes.SUB_M:
                    ExecuteSUB(Memory[Registers.HL]);
                    break;
                case OpcodeBytes.SUB_A:
                    ExecuteSUB(Registers.A);
                    break;
                #endregion

                #region ANA
                case OpcodeBytes.ANA_B:
                    Registers.A = (byte)(Registers.A & Registers.B);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ANA_C:
                    Registers.A = (byte)(Registers.A & Registers.C);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ANA_D:
                    Registers.A = (byte)(Registers.A & Registers.D);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ANA_E:
                    Registers.A = (byte)(Registers.A & Registers.E);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ANA_H:
                    Registers.A = (byte)(Registers.A & Registers.H);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ANA_L:
                    Registers.A = (byte)(Registers.A & Registers.L);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ANA_M:
                    Registers.A = (byte)(Registers.A & Memory[Registers.HL]);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ANA_A:
                    Registers.A = (byte)(Registers.A & Registers.A);
                    SetFlags(false, Registers.A);
                    break;
                #endregion

                #region ORA
                case OpcodeBytes.ORA_B:
                    Registers.A = (byte)(Registers.A | Registers.B);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ORA_C:
                    Registers.A = (byte)(Registers.A | Registers.C);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ORA_D:
                    Registers.A = (byte)(Registers.A | Registers.D);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ORA_E:
                    Registers.A = (byte)(Registers.A | Registers.E);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ORA_H:
                    Registers.A = (byte)(Registers.A | Registers.H);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ORA_L:
                    Registers.A = (byte)(Registers.A | Registers.L);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ORA_M:
                    Registers.A = (byte)(Registers.A | Memory[Registers.HL]);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.ORA_A:
                    Registers.A = (byte)(Registers.A | Registers.A);
                    SetFlags(false, Registers.A);
                    break;
                #endregion

                #region ADC
                case OpcodeBytes.ADC_B:
                    ExecuteADD(Registers.B, true);
                    break;
                case OpcodeBytes.ADC_C:
                    ExecuteADD(Registers.C, true);
                    break;
                case OpcodeBytes.ADC_D:
                    ExecuteADD(Registers.D, true);
                    break;
                case OpcodeBytes.ADC_E:
                    ExecuteADD(Registers.E, true);
                    break;
                case OpcodeBytes.ADC_H:
                    ExecuteADD(Registers.H, true);
                    break;
                case OpcodeBytes.ADC_L:
                    ExecuteADD(Registers.L, true);
                    break;
                case OpcodeBytes.ADC_M:
                    ExecuteADD(Memory[Registers.HL], true);
                    break;
                case OpcodeBytes.ADC_A:
                    ExecuteADD(Registers.A, true);
                    break;
                #endregion

                #region SBB
                case OpcodeBytes.SBB_B:
                    ExecuteSUB(Registers.B, true);
                    break;
                case OpcodeBytes.SBB_C:
                    ExecuteSUB(Registers.C, true);
                    break;
                case OpcodeBytes.SBB_D:
                    ExecuteSUB(Registers.D, true);
                    break;
                case OpcodeBytes.SBB_E:
                    ExecuteSUB(Registers.E, true);
                    break;
                case OpcodeBytes.SBB_H:
                    ExecuteSUB(Registers.H, true);
                    break;
                case OpcodeBytes.SBB_L:
                    ExecuteSUB(Registers.L, true);
                    break;
                case OpcodeBytes.SBB_M:
                    ExecuteSUB(Memory[Registers.HL], true);
                    break;
                case OpcodeBytes.SBB_A:
                    ExecuteSUB(Registers.A, true);
                    break;
                #endregion

                #region XRA
                case OpcodeBytes.XRA_B:
                    Registers.A = (byte)(Registers.A ^ Registers.B);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.XRA_C:
                    Registers.A = (byte)(Registers.A ^ Registers.C);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.XRA_D:
                    Registers.A = (byte)(Registers.A ^ Registers.D);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.XRA_E:
                    Registers.A = (byte)(Registers.A ^ Registers.E);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.XRA_H:
                    Registers.A = (byte)(Registers.A ^ Registers.H);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.XRA_L:
                    Registers.A = (byte)(Registers.A ^ Registers.L);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.XRA_M:
                    Registers.A = (byte)(Registers.A ^ Memory[Registers.HL]);
                    SetFlags(false, Registers.A);
                    break;
                case OpcodeBytes.XRA_A:
                    Registers.A = (byte)(Registers.A ^ Registers.A);
                    SetFlags(false, Registers.A);
                    break;
                #endregion

                #region CMP
                case OpcodeBytes.CMP_B:
                    ExecuteSUB(Registers.B, false, false);
                    break;
                case OpcodeBytes.CMP_C:
                    ExecuteSUB(Registers.C, false, false);
                    break;
                case OpcodeBytes.CMP_D:
                    ExecuteSUB(Registers.D, false, false);
                    break;
                case OpcodeBytes.CMP_E:
                    ExecuteSUB(Registers.E, false, false);
                    break;
                case OpcodeBytes.CMP_H:
                    ExecuteSUB(Registers.H, false, false);
                    break;
                case OpcodeBytes.CMP_L:
                    ExecuteSUB(Registers.L, false, false);
                    break;
                case OpcodeBytes.CMP_M:
                    ExecuteSUB(Memory[Registers.HL], false, false);
                    break;
                case OpcodeBytes.CMP_A:
                    ExecuteSUB(Registers.A, false, false);
                    break;
                #endregion

                #region DAD
                case OpcodeBytes.DAD_B:
                    ExecuteDAD(Registers.BC);
                    break;
                case OpcodeBytes.DAD_D:
                    ExecuteDAD(Registers.DE);
                    break;
                case OpcodeBytes.DAD_H:
                    ExecuteDAD(Registers.HL);
                    break;
                case OpcodeBytes.DAD_SP:
                    ExecuteDAD(StackPointer);
                    break;
                #endregion

                #region INR
                case OpcodeBytes.INR_B:
                    Registers.B++;
                    SetFlags(false, Registers.B);
                    break;
                case OpcodeBytes.INR_C:
                    Registers.C++;
                    SetFlags(false, Registers.C);
                    break;
                case OpcodeBytes.INR_D:
                    Registers.D++;
                    SetFlags(false, Registers.D);
                    break;
                case OpcodeBytes.INR_E:
                    Registers.E++;
                    SetFlags(false, Registers.E);
                    break;
                case OpcodeBytes.INR_H:
                    Registers.H++;
                    SetFlags(false, Registers.H);
                    break;
                case OpcodeBytes.INR_L:
                    Registers.L++;
                    SetFlags(false, Registers.L);
                    break;
                case OpcodeBytes.INR_M:
                    Memory[Registers.HL]++;
                    SetFlags(false, Memory[Registers.HL]);
                    break;
                case OpcodeBytes.INR_A:
                    Registers.A++;
                    SetFlags(false, Registers.A);
                    break;
                #endregion

                #region DCR
                case OpcodeBytes.DCR_B:
                    Registers.B--;
                    SetFlags(false, Registers.B);
                    break;
                case OpcodeBytes.DCR_C:
                    Registers.C--;
                    SetFlags(false, Registers.C);
                    break;
                case OpcodeBytes.DCR_D:
                    Registers.D--;
                    SetFlags(false, Registers.D);
                    break;
                case OpcodeBytes.DCR_E:
                    Registers.E--;
                    SetFlags(false, Registers.E);
                    break;
                case OpcodeBytes.DCR_H:
                    Registers.H--;
                    SetFlags(false, Registers.H);
                    break;
                case OpcodeBytes.DCR_L:
                    Registers.L--;
                    SetFlags(false, Registers.L);
                    break;
                case OpcodeBytes.DCR_M:
                    Memory[Registers.HL]--;
                    SetFlags(false, Memory[Registers.HL]);
                    break;
                case OpcodeBytes.DCR_A:
                    Registers.A--;
                    SetFlags(false, Registers.A);
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

        private void ExecuteMOV(Register dest, Register source)
        {
            Registers[dest] = Registers[source];
        }

        private void ExecuteMOVFromRegisterToMemory(Register source)
        {
            var address = Registers.HL;

            // TODO: Should allow write to memory mirror area?
            // TODO: Should not panic on ROM area write?

            // Only allow writes to the work/video RAM and not the ROM or memory mirror.
            // $2000-$23ff:  work RAM (1K)
            // $2400-$3fff:  video RAM (7K)
            if (address >= 0x2000 && address <= 0x3FFF)
                Memory[address] = Registers[source];
            else
            {
                var addressFormatted = String.Format("0x{0:X4}", address);
                throw new Exception($"Illegal memory address ({addressFormatted}) specified for 'MOV M, {source}' operation; expected address to be between 0x2000 and 0x3FFF inclusive.");
            }
        }

        private void ExecuteMOVIToMemory(byte data)
        {
            var address = Registers.HL;

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

        private void ExecuteADD(byte value, bool addCarryFlag = false)
        {
            var result = Registers.A + value;

            if (addCarryFlag && Flags.Carry)
                result += 1;

            var carryOccurred = result > 255;

            if (carryOccurred)
                result = result - 256;

            SetFlags(carryOccurred, (byte)result);

            Registers.A = (byte)result;
        }

        private void ExecuteSUB(byte value, bool subtractCarryFlag = false, bool updateAccumulator = true)
        {
            var borrowOccurred = (subtractCarryFlag && Flags.Carry)
                ? value >= Registers.A // Account for the extra minus one from the carry flag subtraction.
                : value > Registers.A;

            var result = Registers.A - value;

            if (subtractCarryFlag && Flags.Carry)
                result -= 1;

            if (borrowOccurred)
                result = 256 + result;

            SetFlags(borrowOccurred, (byte)result);

            if (updateAccumulator)
                Registers.A = (byte)result;
        }

        private void ExecuteDAD(UInt16 value)
        {
            var result = Registers.HL + value;

            var carryOccurred = result > 65535;

            if (carryOccurred)
                result = result - 65536;

            Registers.HL = (UInt16)result;

            Flags.Carry = carryOccurred;
        }

        private void SetFlags(bool carry, byte result)
        {
            Flags.Carry = carry;
            Flags.Zero = result == 0;
            Flags.Sign = (result & 0b10000000) == 0b10000000;
            Flags.Parity = CalculateParityBit((byte)result);
            // Flags.AuxCarry = // TODO
        }

        private bool CalculateParityBit(byte value)
        {
            var setBits = 0;

            for (var i = 0; i < 8; i++)
            {
                if ((value & 0x01) == 1)
                    setBits++;

                value = (byte)(value >> 1);
            }

            // Parity bit is set if number of bits is even.
            return setBits == 0 || setBits % 2 == 0;
        }
    }
}
