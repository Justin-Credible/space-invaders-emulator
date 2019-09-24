
using System.Collections.Generic;

namespace JustinCredible.SIEmulator
{
    /**
     * A lookup table for opcodes, which includes opcode sizes and cycle counts.
     * Derived from: http://www.pastraiser.com/cpu/i8080/i8080_opcodes.html
     */
    public class OpcodeTable
    {
        public static Opcode NOP = new Opcode(OpcodeBytes.NOP, 1, "NOP", 4);
        public static Opcode NOP2 = new Opcode(OpcodeBytes.NOP2, 1, "NOP2", 4);
        public static Opcode NOP3 = new Opcode(OpcodeBytes.NOP3, 1, "NOP3", 4);
        public static Opcode NOP4 = new Opcode(OpcodeBytes.NOP4, 1, "NOP4", 4);
        public static Opcode NOP5 = new Opcode(OpcodeBytes.NOP5, 1, "NOP5", 4);
        public static Opcode NOP6 = new Opcode(OpcodeBytes.NOP6, 1, "NOP6", 4);
        public static Opcode NOP7 = new Opcode(OpcodeBytes.NOP7, 1, "NOP7", 4);
        public static Opcode NOP8 = new Opcode(OpcodeBytes.NOP8, 1, "NOP8", 4);

        public static Opcode HLT = new Opcode(OpcodeBytes.HLT, 1, "HLT", 7);

        #region MOV
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

        #region MVI
        public static Opcode MVI_B = new Opcode(OpcodeBytes.MVI_B, 2, "MVI B,D8", 7); // B <- byte 2
        public static Opcode MVI_C = new Opcode(OpcodeBytes.MVI_C, 2, "MVI C,D8", 7); // C <- byte 2
        public static Opcode MVI_D = new Opcode(OpcodeBytes.MVI_D, 2, "MVI D,D8", 7); // D <- byte 2
        public static Opcode MVI_E = new Opcode(OpcodeBytes.MVI_E, 2, "MVI E,D8", 7); // E <- byte 2
        public static Opcode MVI_H = new Opcode(OpcodeBytes.MVI_H, 2, "MVI H,D8", 7); // L <- byte 2
        public static Opcode MVI_L = new Opcode(OpcodeBytes.MVI_L, 2, "MVI L,D8", 7); // L <- byte 2
        public static Opcode MVI_M = new Opcode(OpcodeBytes.MVI_M, 2, "MVI M,D8", 10); // (HL) <- byte 2
        public static Opcode MVI_A = new Opcode(OpcodeBytes.MVI_A, 2, "MVI A,D8", 7); // A <- byte 2
        #endregion

        #region LXI
        public static Opcode LXI_B = new Opcode(OpcodeBytes.LXI_B, 3, "LXI B,D16", 10); // B <- byte 3, C <- byte 2
        public static Opcode LXI_D = new Opcode(OpcodeBytes.LXI_D, 3, "LXI D,D16", 10); // D <- byte 3, E <- byte 2
        public static Opcode LXI_H = new Opcode(OpcodeBytes.LXI_H, 3, "LXI H,D16", 10); // H <- byte 3, L <- byte 2
        public static Opcode LXI_SP = new Opcode(OpcodeBytes.LXI_SP, 3, "LXI SP, D16", 10); // SP.hi <- byte 3, SP.lo <- byte 2
        #endregion

        #region STAX
        public static Opcode STAX_B = new Opcode(OpcodeBytes.STAX_B, 1, "STAX B", 7); // (BC) <- A
        public static Opcode STAX_D = new Opcode(OpcodeBytes.STAX_D, 1, "STAX D", 7); // (DE) <- A
        #endregion

        public static Dictionary<byte, Opcode> Lookup = new Dictionary<byte, Opcode>()
        {
            [OpcodeBytes.NOP] = NOP,
            [OpcodeBytes.HLT] = HLT,

            [OpcodeBytes.LXI_B] = LXI_B,

            #region MOV
            [OpcodeBytes.MOV_B_B] = MOV_B_B,
            [OpcodeBytes.MOV_B_C] = MOV_B_C,
            [OpcodeBytes.MOV_B_D] = MOV_B_D,
            [OpcodeBytes.MOV_B_E] = MOV_B_E,
            [OpcodeBytes.MOV_B_H] = MOV_B_H,
            [OpcodeBytes.MOV_B_L] = MOV_B_L,
            [OpcodeBytes.MOV_B_M] = MOV_B_M,
            [OpcodeBytes.MOV_B_A] = MOV_B_A,
            [OpcodeBytes.MOV_C_B] = MOV_C_B,
            [OpcodeBytes.MOV_C_C] = MOV_C_C,
            [OpcodeBytes.MOV_C_D] = MOV_C_D,
            [OpcodeBytes.MOV_C_E] = MOV_C_E,
            [OpcodeBytes.MOV_C_H] = MOV_C_H,
            [OpcodeBytes.MOV_C_L] = MOV_C_L,
            [OpcodeBytes.MOV_C_M] = MOV_C_M,
            [OpcodeBytes.MOV_C_A] = MOV_C_A,
            [OpcodeBytes.MOV_D_B] = MOV_D_B,
            [OpcodeBytes.MOV_D_C] = MOV_D_C,
            [OpcodeBytes.MOV_D_D] = MOV_D_D,
            [OpcodeBytes.MOV_D_E] = MOV_D_E,
            [OpcodeBytes.MOV_D_H] = MOV_D_H,
            [OpcodeBytes.MOV_D_L] = MOV_D_L,
            [OpcodeBytes.MOV_D_M] = MOV_D_M,
            [OpcodeBytes.MOV_D_A] = MOV_D_A,
            [OpcodeBytes.MOV_E_B] = MOV_E_B,
            [OpcodeBytes.MOV_E_C] = MOV_E_C,
            [OpcodeBytes.MOV_E_D] = MOV_E_D,
            [OpcodeBytes.MOV_E_E] = MOV_E_E,
            [OpcodeBytes.MOV_E_H] = MOV_E_H,
            [OpcodeBytes.MOV_E_L] = MOV_E_L,
            [OpcodeBytes.MOV_E_M] = MOV_E_M,
            [OpcodeBytes.MOV_E_A] = MOV_E_A,
            [OpcodeBytes.MOV_H_B] = MOV_H_B,
            [OpcodeBytes.MOV_H_C] = MOV_H_C,
            [OpcodeBytes.MOV_H_D] = MOV_H_D,
            [OpcodeBytes.MOV_H_E] = MOV_H_E,
            [OpcodeBytes.MOV_H_H] = MOV_H_H,
            [OpcodeBytes.MOV_H_L] = MOV_H_L,
            [OpcodeBytes.MOV_H_M] = MOV_H_M,
            [OpcodeBytes.MOV_H_A] = MOV_H_A,
            [OpcodeBytes.MOV_L_B] = MOV_L_B,
            [OpcodeBytes.MOV_L_C] = MOV_L_C,
            [OpcodeBytes.MOV_L_D] = MOV_L_D,
            [OpcodeBytes.MOV_L_E] = MOV_L_E,
            [OpcodeBytes.MOV_L_H] = MOV_L_H,
            [OpcodeBytes.MOV_L_L] = MOV_L_L,
            [OpcodeBytes.MOV_L_M] = MOV_L_M,
            [OpcodeBytes.MOV_L_A] = MOV_L_A,
            [OpcodeBytes.MOV_M_B] = MOV_M_B,
            [OpcodeBytes.MOV_M_C] = MOV_M_C,
            [OpcodeBytes.MOV_M_D] = MOV_M_D,
            [OpcodeBytes.MOV_M_E] = MOV_M_E,
            [OpcodeBytes.MOV_M_H] = MOV_M_H,
            [OpcodeBytes.MOV_M_L] = MOV_M_L,
            [OpcodeBytes.MOV_M_A] = MOV_M_A,
            [OpcodeBytes.MOV_A_B] = MOV_A_B,
            [OpcodeBytes.MOV_A_C] = MOV_A_C,
            [OpcodeBytes.MOV_A_D] = MOV_A_D,
            [OpcodeBytes.MOV_A_E] = MOV_A_E,
            [OpcodeBytes.MOV_A_H] = MOV_A_H,
            [OpcodeBytes.MOV_A_L] = MOV_A_L,
            [OpcodeBytes.MOV_A_M] = MOV_A_M,
            [OpcodeBytes.MOV_A_A] = MOV_A_A,
            #endregion

            #region MVI
            [OpcodeBytes.MVI_B] = MVI_B,
            [OpcodeBytes.MVI_C] = MVI_C,
            [OpcodeBytes.MVI_D] = MVI_D,
            [OpcodeBytes.MVI_E] = MVI_E,
            [OpcodeBytes.MVI_H] = MVI_H,
            [OpcodeBytes.MVI_L] = MVI_L,
            [OpcodeBytes.MVI_M] = MVI_M,
            [OpcodeBytes.MVI_A] = MVI_A,
            #endregion

            #region LXI
            [OpcodeBytes.LXI_B] = LXI_B,
            [OpcodeBytes.LXI_D] = LXI_D,
            [OpcodeBytes.LXI_H] = LXI_H,
            [OpcodeBytes.LXI_SP] = LXI_SP,
            #endregion

            #region STAX
            [OpcodeBytes.STAX_B] = STAX_B,
            [OpcodeBytes.STAX_D] = STAX_D,
            #endregion
        };
    }
}
