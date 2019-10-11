
using System.Collections.Generic;

namespace JustinCredible.SIEmulator
{
    /**
     * A lookup table for opcodes, which includes opcode sizes and cycle counts.
     * Derived from: http://www.pastraiser.com/cpu/i8080/i8080_opcodes.html
     */
    public partial class Opcodes
    {
        /** Halt */
        public static Opcode HLT = new Opcode(OpcodeBytes.HLT, 1, "HLT", 7);
 
        #region NOP - No operation
            public static Opcode NOP = new Opcode(OpcodeBytes.NOP, 1, "NOP", 4);
            public static Opcode NOP2 = new Opcode(OpcodeBytes.NOP2, 1, "NOP2", 4);
            public static Opcode NOP3 = new Opcode(OpcodeBytes.NOP3, 1, "NOP3", 4);
            public static Opcode NOP4 = new Opcode(OpcodeBytes.NOP4, 1, "NOP4", 4);
            public static Opcode NOP5 = new Opcode(OpcodeBytes.NOP5, 1, "NOP5", 4);
            public static Opcode NOP6 = new Opcode(OpcodeBytes.NOP6, 1, "NOP6", 4);
            public static Opcode NOP7 = new Opcode(OpcodeBytes.NOP7, 1, "NOP7", 4);
            public static Opcode NOP8 = new Opcode(OpcodeBytes.NOP8, 1, "NOP8", 4);
        #endregion

        #region Carry bit instructions

            /** Set Carry */
            public static Opcode STC = new Opcode(OpcodeBytes.STC, 1, "STC", 4); // CY = 1

            /** Complement Carry */
            public static Opcode CMC = new Opcode(OpcodeBytes.CMC, 1, "CMC", 4); // CY=!CY

        #endregion

        #region Single register instructions

        #region INR - Increment Register or Memory
            public static Opcode INR_B = new Opcode(OpcodeBytes.INR_B, 1, "INR B", 5); // B <- B+1
            public static Opcode INR_C = new Opcode(OpcodeBytes.INR_C, 1, "INR C", 5); // C <- C+1
            public static Opcode INR_D = new Opcode(OpcodeBytes.INR_D, 1, "INR D", 5); // D <- D+1
            public static Opcode INR_E = new Opcode(OpcodeBytes.INR_E, 1, "INR E", 5); // E <-E+1
            public static Opcode INR_H = new Opcode(OpcodeBytes.INR_H, 1, "INR H", 5); // H <- H+1
            public static Opcode INR_L = new Opcode(OpcodeBytes.INR_L, 1, "INR L", 5); // L <- L+1
            public static Opcode INR_M = new Opcode(OpcodeBytes.INR_M, 1, "INR M", 10); // (HL) <- (HL)+1
            public static Opcode INR_A = new Opcode(OpcodeBytes.INR_A, 1, "INR A", 5); // A <- A+1
        #endregion

        #region DCR - Decrement Register or Memory
            public static Opcode DCR_B = new Opcode(OpcodeBytes.DCR_B, 1, "DCR B", 5); // B <- B-1
            public static Opcode DCR_C = new Opcode(OpcodeBytes.DCR_C, 1, "DCR C", 5); // C <-C-1
            public static Opcode DCR_D = new Opcode(OpcodeBytes.DCR_D, 1, "DCR D", 5); // D <- D-1
            public static Opcode DCR_E = new Opcode(OpcodeBytes.DCR_E, 1, "DCR E", 5); // E <- E-1
            public static Opcode DCR_H = new Opcode(OpcodeBytes.DCR_H, 1, "DCR H", 5); // H <- H-1
            public static Opcode DCR_L = new Opcode(OpcodeBytes.DCR_L, 1, "DCR L", 5); // L <- L-1
            public static Opcode DCR_M = new Opcode(OpcodeBytes.DCR_M, 1, "DCR M", 10); // (HL) <- (HL)-1
            public static Opcode DCR_A = new Opcode(OpcodeBytes.DCR_A, 1, "DCR A", 5); // A <- A-1
        #endregion

        /** Compliment Accumulator */
        public static Opcode CMA = new Opcode(OpcodeBytes.CMA, 1, "CMA", 4); // A <- !A

        #endregion

        #region Data transfer instructions

            #region STAX - Store accumulator
                public static Opcode STAX_B = new Opcode(OpcodeBytes.STAX_B, 1, "STAX B", 7); // (BC) <- A
                public static Opcode STAX_D = new Opcode(OpcodeBytes.STAX_D, 1, "STAX D", 7); // (DE) <- A
            #endregion

            #region LDAX - Load accumulator
                public static Opcode LDAX_B = new Opcode(OpcodeBytes.LDAX_B, 1, "LDAX B", 7); // A <- (BC)
                public static Opcode LDAX_D = new Opcode(OpcodeBytes.LDAX_D, 1, "LDAX D", 7); // A <- (DE)
            #endregion

            #region MOV - Move (copy) data
                public static Opcode MOV_B_B = new Opcode(OpcodeBytes.MOV_B_B, 1, "MOV B,B", 5); // B <- B
                public static Opcode MOV_B_C = new Opcode(OpcodeBytes.MOV_B_C, 1, "MOV B,C", 5); // B <- C
                public static Opcode MOV_B_D = new Opcode(OpcodeBytes.MOV_B_D, 1, "MOV B,D", 5); // B <- D
                public static Opcode MOV_B_E = new Opcode(OpcodeBytes.MOV_B_E, 1, "MOV B,E", 5); // B <- E
                public static Opcode MOV_B_H = new Opcode(OpcodeBytes.MOV_B_H, 1, "MOV B,H", 5); // B <- H
                public static Opcode MOV_B_L = new Opcode(OpcodeBytes.MOV_B_L, 1, "MOV B,L", 5); // B <- L
                public static Opcode MOV_B_M = new Opcode(OpcodeBytes.MOV_B_M, 1, "MOV B,M", 7); // B <- (HL)
                public static Opcode MOV_B_A = new Opcode(OpcodeBytes.MOV_B_A, 1, "MOV B,A", 5); // B <- A
                public static Opcode MOV_C_B = new Opcode(OpcodeBytes.MOV_C_B, 1, "MOV C,B", 5); // C <- B
                public static Opcode MOV_C_C = new Opcode(OpcodeBytes.MOV_C_C, 1, "MOV C,C", 5); // C <- C
                public static Opcode MOV_C_D = new Opcode(OpcodeBytes.MOV_C_D, 1, "MOV C,D", 5); // C <- D
                public static Opcode MOV_C_E = new Opcode(OpcodeBytes.MOV_C_E, 1, "MOV C,E", 5); // C <- E
                public static Opcode MOV_C_H = new Opcode(OpcodeBytes.MOV_C_H, 1, "MOV C,H", 5); // C <- H
                public static Opcode MOV_C_L = new Opcode(OpcodeBytes.MOV_C_L, 1, "MOV C,L", 5); // C <- L
                public static Opcode MOV_C_M = new Opcode(OpcodeBytes.MOV_C_M, 1, "MOV C,M", 7); // C <- (HL)
                public static Opcode MOV_C_A = new Opcode(OpcodeBytes.MOV_C_A, 1, "MOV C,A", 5); // C <- A
                public static Opcode MOV_D_B = new Opcode(OpcodeBytes.MOV_D_B, 1, "MOV D,B", 5); // D <- B
                public static Opcode MOV_D_C = new Opcode(OpcodeBytes.MOV_D_C, 1, "MOV D,C", 5); // D <- C
                public static Opcode MOV_D_D = new Opcode(OpcodeBytes.MOV_D_D, 1, "MOV D,D", 5); // D <- D
                public static Opcode MOV_D_E = new Opcode(OpcodeBytes.MOV_D_E, 1, "MOV D,E", 5); // D <- E
                public static Opcode MOV_D_H = new Opcode(OpcodeBytes.MOV_D_H, 1, "MOV D,H", 5); // D <- H
                public static Opcode MOV_D_L = new Opcode(OpcodeBytes.MOV_D_L, 1, "MOV D,L", 5); // D <- L
                public static Opcode MOV_D_M = new Opcode(OpcodeBytes.MOV_D_M, 1, "MOV D,M", 7); // D <- (HL)
                public static Opcode MOV_D_A = new Opcode(OpcodeBytes.MOV_D_A, 1, "MOV D,A", 5); // D <- A
                public static Opcode MOV_E_B = new Opcode(OpcodeBytes.MOV_E_B, 1, "MOV E,B", 5); // E <- B
                public static Opcode MOV_E_C = new Opcode(OpcodeBytes.MOV_E_C, 1, "MOV E,C", 5); // E <- C
                public static Opcode MOV_E_D = new Opcode(OpcodeBytes.MOV_E_D, 1, "MOV E,D", 5); // E <- D
                public static Opcode MOV_E_E = new Opcode(OpcodeBytes.MOV_E_E, 1, "MOV E,E", 5); // E <- E
                public static Opcode MOV_E_H = new Opcode(OpcodeBytes.MOV_E_H, 1, "MOV E,H", 5); // E <- H
                public static Opcode MOV_E_L = new Opcode(OpcodeBytes.MOV_E_L, 1, "MOV E,L", 5); // E <- L
                public static Opcode MOV_E_M = new Opcode(OpcodeBytes.MOV_E_M, 1, "MOV E,M", 7); // E <- (HL)
                public static Opcode MOV_E_A = new Opcode(OpcodeBytes.MOV_E_A, 1, "MOV E,A", 5); // E <- A
                public static Opcode MOV_H_B = new Opcode(OpcodeBytes.MOV_H_B, 1, "MOV H,B", 5); // H <- B
                public static Opcode MOV_H_C = new Opcode(OpcodeBytes.MOV_H_C, 1, "MOV H,C", 5); // H <- C
                public static Opcode MOV_H_D = new Opcode(OpcodeBytes.MOV_H_D, 1, "MOV H,D", 5); // H <- D
                public static Opcode MOV_H_E = new Opcode(OpcodeBytes.MOV_H_E, 1, "MOV H,E", 5); // H <- E
                public static Opcode MOV_H_H = new Opcode(OpcodeBytes.MOV_H_H, 1, "MOV H,H", 5); // H <- H
                public static Opcode MOV_H_L = new Opcode(OpcodeBytes.MOV_H_L, 1, "MOV H,L", 5); // H <- L
                public static Opcode MOV_H_M = new Opcode(OpcodeBytes.MOV_H_M, 1, "MOV H,M", 7); // H <- (HL)
                public static Opcode MOV_H_A = new Opcode(OpcodeBytes.MOV_H_A, 1, "MOV H,A", 5); // H <- A
                public static Opcode MOV_L_B = new Opcode(OpcodeBytes.MOV_L_B, 1, "MOV L,B", 5); // L <- B
                public static Opcode MOV_L_C = new Opcode(OpcodeBytes.MOV_L_C, 1, "MOV L,C", 5); // L <- C
                public static Opcode MOV_L_D = new Opcode(OpcodeBytes.MOV_L_D, 1, "MOV L,D", 5); // L <- D
                public static Opcode MOV_L_E = new Opcode(OpcodeBytes.MOV_L_E, 1, "MOV L,E", 5); // L <- E
                public static Opcode MOV_L_H = new Opcode(OpcodeBytes.MOV_L_H, 1, "MOV L,H", 5); // L <- H
                public static Opcode MOV_L_L = new Opcode(OpcodeBytes.MOV_L_L, 1, "MOV L,L", 5); // L <- L
                public static Opcode MOV_L_M = new Opcode(OpcodeBytes.MOV_L_M, 1, "MOV L,M", 7); // L <- (HL)
                public static Opcode MOV_L_A = new Opcode(OpcodeBytes.MOV_L_A, 1, "MOV L,A", 5); // L <- A
                public static Opcode MOV_M_B = new Opcode(OpcodeBytes.MOV_M_B, 1, "MOV M,B", 7); // (HL) <- B
                public static Opcode MOV_M_C = new Opcode(OpcodeBytes.MOV_M_C, 1, "MOV M,C", 7); // (HL) <- C
                public static Opcode MOV_M_D = new Opcode(OpcodeBytes.MOV_M_D, 1, "MOV M,D", 7); // (HL) <- D
                public static Opcode MOV_M_E = new Opcode(OpcodeBytes.MOV_M_E, 1, "MOV M,E", 7); // (HL) <- E
                public static Opcode MOV_M_H = new Opcode(OpcodeBytes.MOV_M_H, 1, "MOV M,H", 7); // (HL) <- H
                public static Opcode MOV_M_L = new Opcode(OpcodeBytes.MOV_M_L, 1, "MOV M,L", 7); // (HL) <- L
                public static Opcode MOV_M_A = new Opcode(OpcodeBytes.MOV_M_A, 1, "MOV M,A", 7); // (HL) <- A
                public static Opcode MOV_A_B = new Opcode(OpcodeBytes.MOV_A_B, 1, "MOV A,B", 5); // A <- B
                public static Opcode MOV_A_C = new Opcode(OpcodeBytes.MOV_A_C, 1, "MOV A,C", 5); // A <- C
                public static Opcode MOV_A_D = new Opcode(OpcodeBytes.MOV_A_D, 1, "MOV A,D", 5); // A <- D
                public static Opcode MOV_A_E = new Opcode(OpcodeBytes.MOV_A_E, 1, "MOV A,E", 5); // A <- E
                public static Opcode MOV_A_H = new Opcode(OpcodeBytes.MOV_A_H, 1, "MOV A,H", 5); // A <- H
                public static Opcode MOV_A_L = new Opcode(OpcodeBytes.MOV_A_L, 1, "MOV A,L", 5); // A <- L
                public static Opcode MOV_A_M = new Opcode(OpcodeBytes.MOV_A_M, 1, "MOV A,M", 7); // A <- (HL)
                public static Opcode MOV_A_A = new Opcode(OpcodeBytes.MOV_A_A, 1, "MOV A,A", 5); // A <- A
            #endregion

        #endregion

        #region Register or memory to accumulator instructions

            #region ADD - Add register or memory to accumulator
                public static Opcode ADD_B = new Opcode(OpcodeBytes.ADD_B, 1, "ADD B", 4); // A <- A + B
                public static Opcode ADD_C = new Opcode(OpcodeBytes.ADD_C, 1, "ADD C", 4); // A <- A + C
                public static Opcode ADD_D = new Opcode(OpcodeBytes.ADD_D, 1, "ADD D", 4); // A <- A + D
                public static Opcode ADD_E = new Opcode(OpcodeBytes.ADD_E, 1, "ADD E", 4); // A <- A + E
                public static Opcode ADD_H = new Opcode(OpcodeBytes.ADD_H, 1, "ADD H", 4); // A <- A + H
                public static Opcode ADD_L = new Opcode(OpcodeBytes.ADD_L, 1, "ADD L", 4); // A <- A + L
                public static Opcode ADD_M = new Opcode(OpcodeBytes.ADD_M, 1, "ADD M", 7); // A <- A + (HL)
                public static Opcode ADD_A = new Opcode(OpcodeBytes.ADD_A, 1, "ADD A", 4); // A <- A + A
            #endregion

            #region SUB - Subtract register or memory from accumulator
                public static Opcode SUB_B = new Opcode(OpcodeBytes.SUB_B, 1, "SUB B", 4); // A <- A - B
                public static Opcode SUB_C = new Opcode(OpcodeBytes.SUB_C, 1, "SUB C", 4); // A <- A - C
                public static Opcode SUB_D = new Opcode(OpcodeBytes.SUB_D, 1, "SUB D", 4); // A <- A - D
                public static Opcode SUB_E = new Opcode(OpcodeBytes.SUB_E, 1, "SUB E", 4); // A <- A - E
                public static Opcode SUB_H = new Opcode(OpcodeBytes.SUB_H, 1, "SUB H", 4); // A <- A - H
                public static Opcode SUB_L = new Opcode(OpcodeBytes.SUB_L, 1, "SUB L", 4); // A <- A - L
                public static Opcode SUB_M = new Opcode(OpcodeBytes.SUB_M, 1, "SUB M", 7); // A <- A - (HL)
                public static Opcode SUB_A = new Opcode(OpcodeBytes.SUB_A, 1, "SUB A", 4); // A <- A - A
            #endregion

            #region ANA - Logical AND register or memory with accumulator
                public static Opcode ANA_B = new Opcode(OpcodeBytes.ANA_B, 1, "ANA B", 4); // A <- A & B
                public static Opcode ANA_C = new Opcode(OpcodeBytes.ANA_C, 1, "ANA C", 4); // A <- A & C
                public static Opcode ANA_D = new Opcode(OpcodeBytes.ANA_D, 1, "ANA D", 4); // A <- A & D
                public static Opcode ANA_E = new Opcode(OpcodeBytes.ANA_E, 1, "ANA E", 4); // A <- A & E
                public static Opcode ANA_H = new Opcode(OpcodeBytes.ANA_H, 1, "ANA H", 4); // A <- A & H
                public static Opcode ANA_L = new Opcode(OpcodeBytes.ANA_L, 1, "ANA L", 4); // A <- A & L
                public static Opcode ANA_M = new Opcode(OpcodeBytes.ANA_M, 1, "ANA M", 7); // A <- A & (HL)
                public static Opcode ANA_A = new Opcode(OpcodeBytes.ANA_A, 1, "ANA A", 4); // A <- A & A
            #endregion

            #region ORA - Logical OR register or memory with accumulator
                public static Opcode ORA_B = new Opcode(OpcodeBytes.ORA_B, 1, "ORA B", 4); // A <- A | B
                public static Opcode ORA_C = new Opcode(OpcodeBytes.ORA_C, 1, "ORA C", 4); // A <- A | C
                public static Opcode ORA_D = new Opcode(OpcodeBytes.ORA_D, 1, "ORA D", 4); // A <- A | D
                public static Opcode ORA_E = new Opcode(OpcodeBytes.ORA_E, 1, "ORA E", 4); // A <- A | E
                public static Opcode ORA_H = new Opcode(OpcodeBytes.ORA_H, 1, "ORA H", 4); // A <- A | H
                public static Opcode ORA_L = new Opcode(OpcodeBytes.ORA_L, 1, "ORA L", 4); // A <- A | L
                public static Opcode ORA_M = new Opcode(OpcodeBytes.ORA_M, 1, "ORA M", 7); // A <- A | (HL)
                public static Opcode ORA_A = new Opcode(OpcodeBytes.ORA_A, 1, "ORA A", 4); // A <- A | A
            #endregion

            #region ADC - Add register or memory to accumulator with carry
                public static Opcode ADC_B = new Opcode(OpcodeBytes.ADC_B, 1, "ADC B", 4); // A <- A + B + CY
                public static Opcode ADC_C = new Opcode(OpcodeBytes.ADC_C, 1, "ADC C", 4); // A <- A + C + CY
                public static Opcode ADC_D = new Opcode(OpcodeBytes.ADC_D, 1, "ADC D", 4); // A <- A + D + CY
                public static Opcode ADC_E = new Opcode(OpcodeBytes.ADC_E, 1, "ADC E", 4); // A <- A + E + CY
                public static Opcode ADC_H = new Opcode(OpcodeBytes.ADC_H, 1, "ADC H", 4); // A <- A + H + CY
                public static Opcode ADC_L = new Opcode(OpcodeBytes.ADC_L, 1, "ADC L", 4); // A <- A + L + CY
                public static Opcode ADC_M = new Opcode(OpcodeBytes.ADC_M, 1, "ADC M", 7); // A <- A + (HL) + CY
                public static Opcode ADC_A = new Opcode(OpcodeBytes.ADC_A, 1, "ADC A", 4); // A <- A + A + CY
            #endregion

            #region SBB - Subtract register or memory from accumulator with borrow
                public static Opcode SBB_B = new Opcode(OpcodeBytes.SBB_B, 1, "SBB B", 4); // A <- A - B - CY
                public static Opcode SBB_C = new Opcode(OpcodeBytes.SBB_C, 1, "SBB C", 4); // A <- A - C - CY
                public static Opcode SBB_D = new Opcode(OpcodeBytes.SBB_D, 1, "SBB D", 4); // A <- A - D - CY
                public static Opcode SBB_E = new Opcode(OpcodeBytes.SBB_E, 1, "SBB E", 4); // A <- A - E - CY
                public static Opcode SBB_H = new Opcode(OpcodeBytes.SBB_H, 1, "SBB H", 4); // A <- A - H - CY
                public static Opcode SBB_L = new Opcode(OpcodeBytes.SBB_L, 1, "SBB L", 4); // A <- A - L - CY
                public static Opcode SBB_M = new Opcode(OpcodeBytes.SBB_M, 1, "SBB M", 7); // A <- A - (HL) - CY
                public static Opcode SBB_A = new Opcode(OpcodeBytes.SBB_A, 1, "SBB A", 4); // A <- A - A - CY
            #endregion

            #region XRA - Logical XOR register or memory with accumulator
                public static Opcode XRA_B = new Opcode(OpcodeBytes.XRA_B, 1, "XRA B", 4); // A <- A ^ B
                public static Opcode XRA_C = new Opcode(OpcodeBytes.XRA_C, 1, "XRA C", 4); // A <- A ^ C
                public static Opcode XRA_D = new Opcode(OpcodeBytes.XRA_D, 1, "XRA D", 4); // A <- A ^ D
                public static Opcode XRA_E = new Opcode(OpcodeBytes.XRA_E, 1, "XRA E", 4); // A <- A ^ E
                public static Opcode XRA_H = new Opcode(OpcodeBytes.XRA_H, 1, "XRA H", 4); // A <- A ^ H
                public static Opcode XRA_L = new Opcode(OpcodeBytes.XRA_L, 1, "XRA L", 4); // A <- A ^ L
                public static Opcode XRA_M = new Opcode(OpcodeBytes.XRA_M, 1, "XRA M", 7); // A <- A ^ (HL)
                public static Opcode XRA_A = new Opcode(OpcodeBytes.XRA_A, 1, "XRA A", 4); // A <- A ^ A
            #endregion

            #region CMP - Compare register or memory with accumulator
                public static Opcode CMP_B = new Opcode(OpcodeBytes.CMP_B, 1, "CMP B", 4); // A - B
                public static Opcode CMP_C = new Opcode(OpcodeBytes.CMP_C, 1, "CMP C", 4); // A - C
                public static Opcode CMP_D = new Opcode(OpcodeBytes.CMP_D, 1, "CMP D", 4); // A - D
                public static Opcode CMP_E = new Opcode(OpcodeBytes.CMP_E, 1, "CMP E", 4); // A - E
                public static Opcode CMP_H = new Opcode(OpcodeBytes.CMP_H, 1, "CMP H", 4); // A - H
                public static Opcode CMP_L = new Opcode(OpcodeBytes.CMP_L, 1, "CMP L", 4); // A - L
                public static Opcode CMP_M = new Opcode(OpcodeBytes.CMP_M, 1, "CMP M", 7); // A - (HL)
                public static Opcode CMP_A = new Opcode(OpcodeBytes.CMP_A, 1, "CMP A", 4); // A - A
            #endregion

            /** Load SP from H and L */
            public static Opcode SPHL = new Opcode(OpcodeBytes.SPHL, 1, "SPHL", 5); // SP=HL

        #endregion

        #region Rotate accumulator instructions

            /** Rotate accumulator left */
            public static Opcode RLC = new Opcode(OpcodeBytes.RLC, 1, "RLC", 4); // A = A << 1; bit 0 = prev bit 7; CY = prev bit 7

            /** Rotate accumulator right */
            public static Opcode RRC = new Opcode(OpcodeBytes.RRC, 1, "RRC", 4); // A = A >> 1; bit 7 = prev bit 0; CY = prev bit 0

            /** Rotate accumulator left through carry */
            public static Opcode RAL = new Opcode(OpcodeBytes.RAL, 1, "RAL", 4); // A = A << 1; bit 0 = prev CY; CY = prev bit 7

            /** Rotate accumulator right through carry */
            public static Opcode RAR = new Opcode(OpcodeBytes.RAR, 1, "RAR", 4); // A = A >> 1; bit 7 = prev bit 7; CY = prev bit 0

        #endregion

        #region Register pair instructions

            #region INX - Increment register pair
                public static Opcode INX_B = new Opcode(OpcodeBytes.INX_B, 1, "INX B", 5); // BC <- BC+1
                public static Opcode INX_D = new Opcode(OpcodeBytes.INX_D, 1, "INX D", 5); // DE <- DE + 1
                public static Opcode INX_H = new Opcode(OpcodeBytes.INX_H, 1, "INX H", 5); // HL <- HL + 1
                public static Opcode INX_SP = new Opcode(OpcodeBytes.INX_SP, 1, "INX SP", 5); // SP = SP + 1
            #endregion

            #region DCX - Decrement register pair
                public static Opcode DCX_B = new Opcode(OpcodeBytes.DCX_B, 1, "DCX B", 5); // BC = BC-1
                public static Opcode DCX_D = new Opcode(OpcodeBytes.DCX_D, 1, "DCX D", 5); // DE = DE-1
                public static Opcode DCX_H = new Opcode(OpcodeBytes.DCX_H, 1, "DCX H", 5); // HL = HL-1
                public static Opcode DCX_SP = new Opcode(OpcodeBytes.DCX_SP, 1, "DCX SP", 5); // SP = SP-1
            #endregion

            #region PUSH - Push data onto the stack
                public static Opcode PUSH_B = new Opcode(OpcodeBytes.PUSH_B, 1, "PUSH B", 11); // (sp-2)<-C; (sp-1)<-B; sp <- sp - 2
                public static Opcode PUSH_D = new Opcode(OpcodeBytes.PUSH_D, 1, "PUSH D", 11); // (sp-2)<-E; (sp-1)<-D; sp <- sp - 2
                public static Opcode PUSH_H = new Opcode(OpcodeBytes.PUSH_H, 1, "PUSH H", 11); // (sp-2)<-L; (sp-1)<-H; sp <- sp - 2
                public static Opcode PUSH_PSW = new Opcode(OpcodeBytes.PUSH_PSW, 1, "PUSH PSW", 11); // (sp-2)<-flags; (sp-1)<-A; sp <- sp - 2
            #endregion

            #region POP - Pop data off of the stack
                public static Opcode POP_B = new Opcode(OpcodeBytes.POP_B, 1, "POP B", 10); // C <- (sp); B <- (sp+1); sp <- sp+2
                public static Opcode POP_D = new Opcode(OpcodeBytes.POP_D, 1, "POP D", 10); // E <- (sp); D <- (sp+1); sp <- sp+2
                public static Opcode POP_H = new Opcode(OpcodeBytes.POP_H, 1, "POP H", 10); // L <- (sp); H <- (sp+1); sp <- sp+2
                public static Opcode POP_PSW = new Opcode(OpcodeBytes.POP_PSW, 1, "POP PSW", 10); // flags <- (sp); A <- (sp+1); sp <- sp+2
            #endregion

            #region DAD - Double (16-bit) add
                public static Opcode DAD_B = new Opcode(OpcodeBytes.DAD_B, 1, "DAD B", 10); // HL = HL + BC
                public static Opcode DAD_D = new Opcode(OpcodeBytes.DAD_D, 1, "DAD D", 10); // HL = HL + DE
                public static Opcode DAD_H = new Opcode(OpcodeBytes.DAD_H, 1, "DAD H", 10); // HL = HL + HL
                public static Opcode DAD_SP = new Opcode(OpcodeBytes.DAD_SP, 1, "DAD SP", 10); // HL = HL + SP
            #endregion

        #endregion

        #region Immediate instructions

            #region MVI - Move immediate data
                public static Opcode MVI_B = new Opcode(OpcodeBytes.MVI_B, 2, "MVI B,D8", 7); // B <- byte 2
                public static Opcode MVI_C = new Opcode(OpcodeBytes.MVI_C, 2, "MVI C,D8", 7); // C <- byte 2
                public static Opcode MVI_D = new Opcode(OpcodeBytes.MVI_D, 2, "MVI D,D8", 7); // D <- byte 2
                public static Opcode MVI_E = new Opcode(OpcodeBytes.MVI_E, 2, "MVI E,D8", 7); // E <- byte 2
                public static Opcode MVI_H = new Opcode(OpcodeBytes.MVI_H, 2, "MVI H,D8", 7); // L <- byte 2
                public static Opcode MVI_L = new Opcode(OpcodeBytes.MVI_L, 2, "MVI L,D8", 7); // L <- byte 2
                public static Opcode MVI_M = new Opcode(OpcodeBytes.MVI_M, 2, "MVI M,D8", 10); // (HL) <- byte 2
                public static Opcode MVI_A = new Opcode(OpcodeBytes.MVI_A, 2, "MVI A,D8", 7); // A <- byte 2
            #endregion

            #region LXI - Load register pair immediate
                public static Opcode LXI_B = new Opcode(OpcodeBytes.LXI_B, 3, "LXI B,D16", 10); // B <- byte 3, C <- byte 2
                public static Opcode LXI_D = new Opcode(OpcodeBytes.LXI_D, 3, "LXI D,D16", 10); // D <- byte 3, E <- byte 2
                public static Opcode LXI_H = new Opcode(OpcodeBytes.LXI_H, 3, "LXI H,D16", 10); // H <- byte 3, L <- byte 2
                public static Opcode LXI_SP = new Opcode(OpcodeBytes.LXI_SP, 3, "LXI SP, D16", 10); // SP.hi <- byte 3, SP.lo <- byte 2
            #endregion

            /** Add immediate to accumulator */
            public static Opcode ADI =new Opcode(OpcodeBytes.ADI, 2, "ADI D8", 7); // A <- A + byte

            /** Add immediate to accumulator with carry */
            public static Opcode ACI =new Opcode(OpcodeBytes.ACI, 2, "ACI D8", 7); // A <- A + data + CY

            /** Subtract immediate from accumulator */
            public static Opcode SUI =new Opcode(OpcodeBytes.SUI, 2, "SUI D8", 7); // A <- A - data

            /** Subtract immediate from accumulator with borrow */
            public static Opcode SBI =new Opcode(OpcodeBytes.SBI, 2, "SBI D8", 7); // A <- A - data - CY

            /** Logical AND immediate with accumulator */
            public static Opcode ANI =new Opcode(OpcodeBytes.ANI, 2, "ANI D8", 7); // A <- A & data

            /** XOR immediate with accumulator */
            public static Opcode XRI =new Opcode(OpcodeBytes.XRI, 2, "XRI D8", 7); // A <- A ^ data

            /** Logical OR immediate with accumulator */
            public static Opcode ORI =new Opcode(OpcodeBytes.ORI, 2, "ORI D8", 7); // A <- A | data

            /** Compare immediate with accumulator */
            public static Opcode CPI =new Opcode(OpcodeBytes.CPI, 2, "CPI D8", 7); // A - data

        #endregion

        #region Direct addressing instructions

            /** Store accumulator direct */
            public static Opcode STA = new Opcode(OpcodeBytes.STA, 3, "STA adr", 13); // (adr) <- A

            /** Load accumulator direct */
            public static Opcode LDA = new Opcode(OpcodeBytes.LDA, 3, "LDA adr", 13); // A <- (adr)

            /** Store H and L direct */
            public static Opcode SHLD = new Opcode(OpcodeBytes.SHLD, 3, "SHLD adr", 16); // (adr) <-L; (adr+1)<-H

            /** Load H and L direct */
            public static Opcode LHLD = new Opcode(OpcodeBytes.LHLD, 3, "LHLD adr", 16); // L <- (adr); H<-(adr+1)

        #endregion

        #region Jump instructions

            /** Load program counter */
            public static Opcode PCHL = new Opcode(OpcodeBytes.PCHL, 1, "PCHL", 5); // PC.hi <- H; PC.lo <- L

            /** Jump */
            public static Opcode JMP = new Opcode(OpcodeBytes.JMP, 3, "JMP adr", 10); // PC <= adr

            /** Jump (duplicate) */
            public static Opcode JMP2 = new Opcode(OpcodeBytes.JMP2, 3, "JMP adr", 10); // PC <= adr

            /** Jump if parity odd */
            public static Opcode JPO = new Opcode(OpcodeBytes.JPO, 3, "JPO adr", 10); // if PO, PC <- adr

            /** Jump if parity even */
            public static Opcode JPE = new Opcode(OpcodeBytes.JPE, 3, "JPE adr", 10); // if PE, PC <- adr

            /** Jump if plus/positive */
            public static Opcode JP = new Opcode(OpcodeBytes.JP, 3, "JP adr", 10); // if P=1 PC <- adr

            /** Jump if zero */
            public static Opcode JZ = new Opcode(OpcodeBytes.JZ, 3, "JZ adr", 10); // if Z, PC <- adr

            /** Jump if not zero */
            public static Opcode JNZ = new Opcode(OpcodeBytes.JNZ, 3, "JNZ adr", 10); // if NZ, PC <- adr

            /** Jump if not carry */
            public static Opcode JNC = new Opcode(OpcodeBytes.JNC, 3, "JNC adr", 10); // if NCY, PC<-adr

            /** Jump if carry */
            public static Opcode JC = new Opcode(OpcodeBytes.JC, 3, "JC adr", 10); // if CY, PC<-adr

            /** Jump if minus/negative */
            public static Opcode JM = new Opcode(OpcodeBytes.JM, 3, "JM adr", 10); // if M, PC <- adr

        #endregion

        #region Call subroutine instructions

            public static Opcode CALL = new Opcode(OpcodeBytes.CALL, 3, "CALL adr", 17); // (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
            public static Opcode CALL2 = new Opcode(OpcodeBytes.CALL2, 3, "CALL adr", 17); // (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
            public static Opcode CALL3 = new Opcode(OpcodeBytes.CALL3, 3, "CALL adr", 17); // (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
            public static Opcode CALL4 = new Opcode(OpcodeBytes.CALL4, 3, "CALL adr", 17); // (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr

            /** Call if minus/negative */
            public static Opcode CM = new Opcode(OpcodeBytes.CM, 3, "CM adr", 17, 11); // if M, CALL adr

            /** Call if party even */
            public static Opcode CPE = new Opcode(OpcodeBytes.CPE, 3, "CPE adr", 17, 11); // if PE, CALL adr

            /** Call if carry */
            public static Opcode CC = new Opcode(OpcodeBytes.CC, 3, "CC adr", 17, 11); // if CY, CALL adr

            /** Call if zero */
            public static Opcode CZ = new Opcode(OpcodeBytes.CZ, 3, "CZ adr", 17, 11); // if Z, CALL adr

            /** Call if plus/positive */
            public static Opcode CP = new Opcode(OpcodeBytes.CP, 3, "CP adr", 17, 11); // if P, PC <- adr

            /** Call if party odd */
            public static Opcode CPO = new Opcode(OpcodeBytes.CPO, 3, "CPO adr", 17, 11); // if PO, CALL adr

            /** Call if no carry */
            public static Opcode CNC = new Opcode(OpcodeBytes.CNC, 3, "CNC adr", 17, 11); // if NCY, CALL adr

            /** Call if not zero */
            public static Opcode CNZ = new Opcode(OpcodeBytes.CNZ, 3, "CNZ adr", 17, 11); // if NZ, CALL adr

        #endregion

        #region Return from subroutine instructions

            /** Return from subroutine */
            public static Opcode RET = new Opcode(OpcodeBytes.RET, 1, "RET", 10); // PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2

            /** Return from subroutine (duplicate) */
            public static Opcode RET2 = new Opcode(OpcodeBytes.RET2, 1, "RET2", 10); // PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2

            /** Return if not zero */
            public static Opcode RNZ = new Opcode(OpcodeBytes.RNZ, 1, "RNZ", 11, 5); // if NZ, RET

            /** Return if zero */
            public static Opcode RZ = new Opcode(OpcodeBytes.RZ, 1, "RZ", 11, 5); // if Z, RET

            /** Return if no carry */
            public static Opcode RNC = new Opcode(OpcodeBytes.RNC, 1, "RNC", 11, 5); // if NCY, RET

            /** Return if carry */
            public static Opcode RC = new Opcode(OpcodeBytes.RC, 1, "RC", 11, 5); // if CY, RET

            /** Return if parity odd */
            public static Opcode RPO = new Opcode(OpcodeBytes.RPO, 1, "RPO", 11, 5); // if PO, RET

            /** Return if parity even */
            public static Opcode RPE = new Opcode(OpcodeBytes.RPE, 1, "RPE", 11, 5); // if PE, RET

            /** Return if plus/positive */
            public static Opcode RP = new Opcode(OpcodeBytes.RP, 1, "RP", 11, 5); // if P, RET

            /** Return if minus/negative */
            public static Opcode RM = new Opcode(OpcodeBytes.RM, 1, "RM", 11, 5); // if M, RET


        #endregion

        #region Restart (interrupt handlers) instructions

            /** CALL $0 */
            public static Opcode RST_0 = new Opcode(OpcodeBytes.RST_0, 1, "RST 0", 11); // CALL $0

            /** CALL $8 */
            public static Opcode RST_1 = new Opcode(OpcodeBytes.RST_1, 1, "RST 1", 11); // CALL $8

            /** CALL $10 */
            public static Opcode RST_2 = new Opcode(OpcodeBytes.RST_2, 1, "RST 2", 11); // CALL $10

            /** CALL $18 */
            public static Opcode RST_3 = new Opcode(OpcodeBytes.RST_3, 1, "RST 3", 11); // CALL $18

            /** CALL $20 */
            public static Opcode RST_4 = new Opcode(OpcodeBytes.RST_4, 1, "RST 4", 11); // CALL $20

            /** CALL $28 */
            public static Opcode RST_5 = new Opcode(OpcodeBytes.RST_5, 1, "RST 5", 11); // CALL $28

            /** CALL $30 */
            public static Opcode RST_6 = new Opcode(OpcodeBytes.RST_6, 1, "RST 6", 11); // CALL $30

            /** CALL $38 */
            public static Opcode RST_7 = new Opcode(OpcodeBytes.RST_7, 1, "RST 7", 11); // CALL $38

        #endregion
    }
}
