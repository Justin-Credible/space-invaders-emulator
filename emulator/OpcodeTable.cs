
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

        #region JUMP

        public static Opcode JMP = new Opcode(OpcodeBytes.JMP, 3, "JMP adr", 10); // PC <= adr
        public static Opcode JMP2 = new Opcode(OpcodeBytes.JMP2, 3, "JMP adr", 10); // PC <= adr

        public static Opcode PCHL = new Opcode(OpcodeBytes.PCHL, 1, "PCHL", 5); // PC.hi <- H; PC.lo <- L
        public static Opcode JPO = new Opcode(OpcodeBytes.JPO, 3, "JPO adr", 10); // if PO, PC <- adr
        public static Opcode JPE = new Opcode(OpcodeBytes.JPE, 3, "JPE adr", 10); // if PE, PC <- adr
        public static Opcode JP = new Opcode(OpcodeBytes.JP, 3, "JP adr", 10); // if P=1 PC <- adr
        public static Opcode JZ = new Opcode(OpcodeBytes.JZ, 3, "JZ adr", 10); // if Z, PC <- adr
        public static Opcode JNZ = new Opcode(OpcodeBytes.JNZ, 3, "JNZ adr", 10); // if NZ, PC <- adr
        public static Opcode JNC = new Opcode(OpcodeBytes.JNC, 3, "JNC adr", 10); // if NCY, PC<-adr
        public static Opcode JC = new Opcode(OpcodeBytes.JC, 3, "JC adr", 10); // if CY, PC<-adr
        public static Opcode JM = new Opcode(OpcodeBytes.JM, 3, "JM adr", 10); // if M, PC <- adr

        #endregion

        #region CALL

        public static Opcode CALL = new Opcode(OpcodeBytes.CALL, 3, "CALL adr", 17); // (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
        public static Opcode CALL2 = new Opcode(OpcodeBytes.CALL2, 3, "CALL adr", 17); // (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
        public static Opcode CALL3 = new Opcode(OpcodeBytes.CALL3, 3, "CALL adr", 17); // (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr
        public static Opcode CALL4 = new Opcode(OpcodeBytes.CALL4, 3, "CALL adr", 17); // (SP-1)<-PC.hi;(SP-2)<-PC.lo;SP<-SP-2;PC=adr

        public static Opcode CM = new Opcode(OpcodeBytes.CM, 3, "CM adr", 17, 11); // if M, CALL adr
        public static Opcode CPE = new Opcode(OpcodeBytes.CPE, 3, "CPE adr", 17, 11); // if PE, CALL adr
        public static Opcode CC = new Opcode(OpcodeBytes.CC, 3, "CC adr", 17, 11); // if CY, CALL adr
        public static Opcode CZ = new Opcode(OpcodeBytes.CZ, 3, "CZ adr", 17, 11); // if Z, CALL adr
        public static Opcode CP = new Opcode(OpcodeBytes.CP, 3, "CP adr", 17, 11); // if P, PC <- adr
        public static Opcode CPO = new Opcode(OpcodeBytes.CPO, 3, "CPO adr", 17, 11); // if PO, CALL adr
        public static Opcode CNC = new Opcode(OpcodeBytes.CNC, 3, "CNC adr", 17, 11); // if NCY, CALL adr
        public static Opcode CNZ = new Opcode(OpcodeBytes.CNZ, 3, "CNZ adr", 17, 11); // if NZ, CALL adr

        #endregion

        public static Opcode RET = new Opcode(OpcodeBytes.RET, 1, "RET", 10); // PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2
        public static Opcode RET2 = new Opcode(OpcodeBytes.RET2, 1, "RET2", 10); // PC.lo <- (sp); PC.hi<-(sp+1); SP <- SP+2

        public static Opcode STA = new Opcode(OpcodeBytes.HLT, 3, "STA adr", 13); // (adr) <- A
        public static Opcode LDA = new Opcode(OpcodeBytes.LDA, 3, "LDA adr", 13); // A <- (adr)
        public static Opcode CMA = new Opcode(OpcodeBytes.CMC, 1, "CMA", 4); // A <- !A

        public static Opcode STC = new Opcode(OpcodeBytes.HLT, 1, "STC", 4); // CY = 1
        public static Opcode CMC = new Opcode(OpcodeBytes.CMC, 1, "CMC", 4); // CY=!CY

        public static Opcode SHLD = new Opcode(OpcodeBytes.SHLD, 3, "SHLD adr", 16); // (adr) <-L; (adr+1)<-H
        public static Opcode LHLD = new Opcode(OpcodeBytes.LHLD, 3, "LHLD adr", 16); // L <- (adr); H<-(adr+1)

        public static Opcode RLC = new Opcode(OpcodeBytes.RLC, 1, "RLC", 4); // A = A << 1; bit 0 = prev bit 7; CY = prev bit 7
        public static Opcode RRC = new Opcode(OpcodeBytes.RRC, 1, "RRC", 4); // A = A >> 1; bit 7 = prev bit 0; CY = prev bit 0
        public static Opcode RAL = new Opcode(OpcodeBytes.RAL, 1, "RAL", 4); // A = A << 1; bit 0 = prev CY; CY = prev bit 7
        public static Opcode RAR = new Opcode(OpcodeBytes.RAR, 1, "RAR", 4); // A = A >> 1; bit 7 = prev bit 7; CY = prev bit 0

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

        #region LDAX
        public static Opcode LDAX_B = new Opcode(OpcodeBytes.LDAX_B, 1, "LDAX B", 7); // A <- (BC)
        public static Opcode LDAX_D = new Opcode(OpcodeBytes.LDAX_D, 1, "LDAX D", 7); // A <- (DE)
        #endregion

        #region INX
        public static Opcode INX_B = new Opcode(OpcodeBytes.INX_B, 1, "INX B", 5); // BC <- BC+1
        public static Opcode INX_D = new Opcode(OpcodeBytes.INX_D, 1, "INX D", 5); // DE <- DE + 1
        public static Opcode INX_H = new Opcode(OpcodeBytes.INX_H, 1, "INX H", 5); // HL <- HL + 1
        public static Opcode INX_SP = new Opcode(OpcodeBytes.INX_SP, 1, "INX SP", 5); // SP = SP + 1
        #endregion

        #region DCX
        public static Opcode DCX_B = new Opcode(OpcodeBytes.DCX_B, 1, "DCX B", 5); // BC = BC-1
        public static Opcode DCX_D = new Opcode(OpcodeBytes.DCX_D, 1, "DCX D", 5); // DE = DE-1
        public static Opcode DCX_H = new Opcode(OpcodeBytes.DCX_H, 1, "DCX H", 5); // HL = HL-1
        public static Opcode DCX_SP = new Opcode(OpcodeBytes.DCX_SP, 1, "DCX SP", 5); // SP = SP-1
        #endregion

        #region PUSH
        public static Opcode PUSH_B = new Opcode(OpcodeBytes.PUSH_B, 1, "PUSH B", 11); // (sp-2)<-C; (sp-1)<-B; sp <- sp - 2
        public static Opcode PUSH_D = new Opcode(OpcodeBytes.PUSH_D, 1, "PUSH D", 11); // (sp-2)<-E; (sp-1)<-D; sp <- sp - 2
        public static Opcode PUSH_H = new Opcode(OpcodeBytes.PUSH_H, 1, "PUSH H", 11); // (sp-2)<-L; (sp-1)<-H; sp <- sp - 2
        public static Opcode PUSH_PSW = new Opcode(OpcodeBytes.PUSH_PSW, 1, "PUSH PSW", 11); // (sp-2)<-flags; (sp-1)<-A; sp <- sp - 2
        #endregion

        #region POP
        public static Opcode POP_B = new Opcode(OpcodeBytes.POP_B, 1, "POP B", 10); // C <- (sp); B <- (sp+1); sp <- sp+2
        public static Opcode POP_D = new Opcode(OpcodeBytes.POP_D, 1, "POP D", 10); // E <- (sp); D <- (sp+1); sp <- sp+2
        public static Opcode POP_H = new Opcode(OpcodeBytes.POP_H, 1, "POP H", 10); // L <- (sp); H <- (sp+1); sp <- sp+2
        public static Opcode POP_PSW = new Opcode(OpcodeBytes.POP_PSW, 1, "POP PSW", 10); // flags <- (sp); A <- (sp+1); sp <- sp+2
        #endregion

        #region ADD
        public static Opcode ADD_B = new Opcode(OpcodeBytes.ADD_B, 1, "ADD B", 4); // A <- A + B
        public static Opcode ADD_C = new Opcode(OpcodeBytes.ADD_C, 1, "ADD C", 4); // A <- A + C
        public static Opcode ADD_D = new Opcode(OpcodeBytes.ADD_D, 1, "ADD D", 4); // A <- A + D
        public static Opcode ADD_E = new Opcode(OpcodeBytes.ADD_E, 1, "ADD E", 4); // A <- A + E
        public static Opcode ADD_H = new Opcode(OpcodeBytes.ADD_H, 1, "ADD H", 4); // A <- A + H
        public static Opcode ADD_L = new Opcode(OpcodeBytes.ADD_L, 1, "ADD L", 4); // A <- A + L
        public static Opcode ADD_M = new Opcode(OpcodeBytes.ADD_M, 1, "ADD M", 7); // A <- A + (HL)
        public static Opcode ADD_A = new Opcode(OpcodeBytes.ADD_A, 1, "ADD A", 4); // A <- A + A
        #endregion

        #region SUB
        public static Opcode SUB_B = new Opcode(OpcodeBytes.SUB_B, 1, "SUB B", 4); // A <- A - B
        public static Opcode SUB_C = new Opcode(OpcodeBytes.SUB_C, 1, "SUB C", 4); // A <- A - C
        public static Opcode SUB_D = new Opcode(OpcodeBytes.SUB_D, 1, "SUB D", 4); // A <- A - D
        public static Opcode SUB_E = new Opcode(OpcodeBytes.SUB_E, 1, "SUB E", 4); // A <- A - E
        public static Opcode SUB_H = new Opcode(OpcodeBytes.SUB_H, 1, "SUB H", 4); // A <- A - H
        public static Opcode SUB_L = new Opcode(OpcodeBytes.SUB_L, 1, "SUB L", 4); // A <- A - L
        public static Opcode SUB_M = new Opcode(OpcodeBytes.SUB_M, 1, "SUB M", 7); // A <- A - (HL)
        public static Opcode SUB_A = new Opcode(OpcodeBytes.SUB_A, 1, "SUB A", 4); // A <- A - A
        #endregion

        #region ANA
        public static Opcode ANA_B = new Opcode(OpcodeBytes.ANA_B, 1, "ANA B", 4); // A <- A & B
        public static Opcode ANA_C = new Opcode(OpcodeBytes.ANA_C, 1, "ANA C", 4); // A <- A & C
        public static Opcode ANA_D = new Opcode(OpcodeBytes.ANA_D, 1, "ANA D", 4); // A <- A & D
        public static Opcode ANA_E = new Opcode(OpcodeBytes.ANA_E, 1, "ANA E", 4); // A <- A & E
        public static Opcode ANA_H = new Opcode(OpcodeBytes.ANA_H, 1, "ANA H", 4); // A <- A & H
        public static Opcode ANA_L = new Opcode(OpcodeBytes.ANA_L, 1, "ANA L", 4); // A <- A & L
        public static Opcode ANA_M = new Opcode(OpcodeBytes.ANA_M, 1, "ANA M", 7); // A <- A & (HL)
        public static Opcode ANA_A = new Opcode(OpcodeBytes.ANA_A, 1, "ANA A", 4); // A <- A & A
        #endregion

        #region ORA
        public static Opcode ORA_B = new Opcode(OpcodeBytes.ORA_B, 1, "ORA B", 4); // A <- A | B
        public static Opcode ORA_C = new Opcode(OpcodeBytes.ORA_C, 1, "ORA C", 4); // A <- A | C
        public static Opcode ORA_D = new Opcode(OpcodeBytes.ORA_D, 1, "ORA D", 4); // A <- A | D
        public static Opcode ORA_E = new Opcode(OpcodeBytes.ORA_E, 1, "ORA E", 4); // A <- A | E
        public static Opcode ORA_H = new Opcode(OpcodeBytes.ORA_H, 1, "ORA H", 4); // A <- A | H
        public static Opcode ORA_L = new Opcode(OpcodeBytes.ORA_L, 1, "ORA L", 4); // A <- A | L
        public static Opcode ORA_M = new Opcode(OpcodeBytes.ORA_M, 1, "ORA M", 7); // A <- A | (HL)
        public static Opcode ORA_A = new Opcode(OpcodeBytes.ORA_A, 1, "ORA A", 4); // A <- A | A
        #endregion

        #region ADC
        public static Opcode ADC_B = new Opcode(OpcodeBytes.ADC_B, 1, "ADC B", 4); // A <- A + B + CY
        public static Opcode ADC_C = new Opcode(OpcodeBytes.ADC_C, 1, "ADC C", 4); // A <- A + C + CY
        public static Opcode ADC_D = new Opcode(OpcodeBytes.ADC_D, 1, "ADC D", 4); // A <- A + D + CY
        public static Opcode ADC_E = new Opcode(OpcodeBytes.ADC_E, 1, "ADC E", 4); // A <- A + E + CY
        public static Opcode ADC_H = new Opcode(OpcodeBytes.ADC_H, 1, "ADC H", 4); // A <- A + H + CY
        public static Opcode ADC_L = new Opcode(OpcodeBytes.ADC_L, 1, "ADC L", 4); // A <- A + L + CY
        public static Opcode ADC_M = new Opcode(OpcodeBytes.ADC_M, 1, "ADC M", 7); // A <- A + (HL) + CY
        public static Opcode ADC_A = new Opcode(OpcodeBytes.ADC_A, 1, "ADC A", 4); // A <- A + A + CY
        #endregion

        #region SBB
        public static Opcode SBB_B = new Opcode(OpcodeBytes.SBB_B, 1, "SBB B", 4); // A <- A - B - CY
        public static Opcode SBB_C = new Opcode(OpcodeBytes.SBB_C, 1, "SBB C", 4); // A <- A - C - CY
        public static Opcode SBB_D = new Opcode(OpcodeBytes.SBB_D, 1, "SBB D", 4); // A <- A - D - CY
        public static Opcode SBB_E = new Opcode(OpcodeBytes.SBB_E, 1, "SBB E", 4); // A <- A - E - CY
        public static Opcode SBB_H = new Opcode(OpcodeBytes.SBB_H, 1, "SBB H", 4); // A <- A - H - CY
        public static Opcode SBB_L = new Opcode(OpcodeBytes.SBB_L, 1, "SBB L", 4); // A <- A - L - CY
        public static Opcode SBB_M = new Opcode(OpcodeBytes.SBB_M, 1, "SBB M", 7); // A <- A - (HL) - CY
        public static Opcode SBB_A = new Opcode(OpcodeBytes.SBB_A, 1, "SBB A", 4); // A <- A - A - CY
        #endregion

        #region XRA
        public static Opcode XRA_B = new Opcode(OpcodeBytes.XRA_B, 1, "XRA B", 4); // A <- A ^ B
        public static Opcode XRA_C = new Opcode(OpcodeBytes.XRA_C, 1, "XRA C", 4); // A <- A ^ C
        public static Opcode XRA_D = new Opcode(OpcodeBytes.XRA_D, 1, "XRA D", 4); // A <- A ^ D
        public static Opcode XRA_E = new Opcode(OpcodeBytes.XRA_E, 1, "XRA E", 4); // A <- A ^ E
        public static Opcode XRA_H = new Opcode(OpcodeBytes.XRA_H, 1, "XRA H", 4); // A <- A ^ H
        public static Opcode XRA_L = new Opcode(OpcodeBytes.XRA_L, 1, "XRA L", 4); // A <- A ^ L
        public static Opcode XRA_M = new Opcode(OpcodeBytes.XRA_M, 1, "XRA M", 7); // A <- A ^ (HL)
        public static Opcode XRA_A = new Opcode(OpcodeBytes.XRA_A, 1, "XRA A", 4); // A <- A ^ A
        #endregion

        #region CMP
        public static Opcode CMP_B = new Opcode(OpcodeBytes.CMP_B, 1, "CMP B", 4); // A - B
        public static Opcode CMP_C = new Opcode(OpcodeBytes.CMP_C, 1, "CMP C", 4); // A - C
        public static Opcode CMP_D = new Opcode(OpcodeBytes.CMP_D, 1, "CMP D", 4); // A - D
        public static Opcode CMP_E = new Opcode(OpcodeBytes.CMP_E, 1, "CMP E", 4); // A - E
        public static Opcode CMP_H = new Opcode(OpcodeBytes.CMP_H, 1, "CMP H", 4); // A - H
        public static Opcode CMP_L = new Opcode(OpcodeBytes.CMP_L, 1, "CMP L", 4); // A - L
        public static Opcode CMP_M = new Opcode(OpcodeBytes.CMP_M, 1, "CMP M", 7); // A - (HL)
        public static Opcode CMP_A = new Opcode(OpcodeBytes.CMP_A, 1, "CMP A", 4); // A - A
        #endregion

        #region DAD
        public static Opcode DAD_B = new Opcode(OpcodeBytes.DAD_B, 1, "DAD B", 10); // HL = HL + BC
        public static Opcode DAD_D = new Opcode(OpcodeBytes.DAD_D, 1, "DAD D", 10); // HL = HL + DE
        public static Opcode DAD_H = new Opcode(OpcodeBytes.DAD_H, 1, "DAD H", 10); // HL = HL + HL
        public static Opcode DAD_SP = new Opcode(OpcodeBytes.DAD_SP, 1, "DAD SP", 10); // HL = HL + SP
        #endregion

        #region INR
        public static Opcode INR_B = new Opcode(OpcodeBytes.INR_B, 1, "INR B", 5); // B <- B+1
        public static Opcode INR_C = new Opcode(OpcodeBytes.INR_C, 1, "INR C", 5); // C <- C+1
        public static Opcode INR_D = new Opcode(OpcodeBytes.INR_D, 1, "INR D", 5); // D <- D+1
        public static Opcode INR_E = new Opcode(OpcodeBytes.INR_E, 1, "INR E", 5); // E <-E+1
        public static Opcode INR_H = new Opcode(OpcodeBytes.INR_H, 1, "INR H", 5); // H <- H+1
        public static Opcode INR_L = new Opcode(OpcodeBytes.INR_L, 1, "INR L", 5); // L <- L+1
        public static Opcode INR_M = new Opcode(OpcodeBytes.INR_M, 1, "INR M", 10); // (HL) <- (HL)+1
        public static Opcode INR_A = new Opcode(OpcodeBytes.INR_A, 1, "INR A", 5); // A <- A+1
        #endregion

        #region DCR
        public static Opcode DCR_B = new Opcode(OpcodeBytes.DCR_B, 1, "DCR B", 5); // B <- B-1
        public static Opcode DCR_C = new Opcode(OpcodeBytes.DCR_C, 1, "DCR C", 5); // C <-C-1
        public static Opcode DCR_D = new Opcode(OpcodeBytes.DCR_D, 1, "DCR D", 5); // D <- D-1
        public static Opcode DCR_E = new Opcode(OpcodeBytes.DCR_E, 1, "DCR E", 5); // E <- E-1
        public static Opcode DCR_H = new Opcode(OpcodeBytes.DCR_H, 1, "DCR H", 5); // H <- H-1
        public static Opcode DCR_L = new Opcode(OpcodeBytes.DCR_L, 1, "DCR L", 5); // L <- L-1
        public static Opcode DCR_M = new Opcode(OpcodeBytes.DCR_M, 1, "DCR M", 10); // (HL) <- (HL)-1
        public static Opcode DCR_A = new Opcode(OpcodeBytes.DCR_A, 1, "DCR A", 5); // A <- A-1
        #endregion

        public static Dictionary<byte, Opcode> Lookup = new Dictionary<byte, Opcode>()
        {
            [OpcodeBytes.NOP] = NOP,
            [OpcodeBytes.NOP2] = NOP2,
            [OpcodeBytes.NOP3] = NOP3,
            [OpcodeBytes.NOP4] = NOP4,
            [OpcodeBytes.NOP5] = NOP5,
            [OpcodeBytes.NOP6] = NOP6,
            [OpcodeBytes.NOP7] = NOP7,
            [OpcodeBytes.NOP8] = NOP8,

            [OpcodeBytes.HLT] = HLT,

            #region JUMP

            [OpcodeBytes.JMP] = JMP,
            [OpcodeBytes.JMP2] = JMP2,

            [OpcodeBytes.PCHL] = PCHL,
            [OpcodeBytes.JPO] = JPO,
            [OpcodeBytes.JPE] = JPE,
            [OpcodeBytes.JP] = JP,
            [OpcodeBytes.JZ] = JZ,
            [OpcodeBytes.JNZ] = JNZ,
            [OpcodeBytes.JNC] = JNC,
            [OpcodeBytes.JC] = JC,
            [OpcodeBytes.JM] = JM,

            #endregion

            #region CALL

            [OpcodeBytes.CALL] = CALL,
            [OpcodeBytes.CALL2] = CALL2,
            [OpcodeBytes.CALL3] = CALL3,
            [OpcodeBytes.CALL4] = CALL4,

            [OpcodeBytes.CM] = CM,
            [OpcodeBytes.CPE] = CPE,
            [OpcodeBytes.CC] = CC,
            [OpcodeBytes.CZ] = CZ,
            [OpcodeBytes.CP] = CP,
            [OpcodeBytes.CPO] = CPO,
            [OpcodeBytes.CNC] = CNC,
            [OpcodeBytes.CNZ] = CNZ,

            #endregion

            [OpcodeBytes.RET] = RET,
            [OpcodeBytes.RET2] = RET2,

            [OpcodeBytes.STA] = STA,
            [OpcodeBytes.LDA] = LDA,
            [OpcodeBytes.CMA] = CMA,

            [OpcodeBytes.STC] = STC,
            [OpcodeBytes.CMC] = CMC,

            [OpcodeBytes.SHLD] = SHLD,
            [OpcodeBytes.LHLD] = LHLD,

            [OpcodeBytes.RLC] = RLC,
            [OpcodeBytes.RRC] = RRC,
            [OpcodeBytes.RAL] = RAL,
            [OpcodeBytes.RAR] = RAR,

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

            #region LDAX
            [OpcodeBytes.LDAX_B] = LDAX_B,
            [OpcodeBytes.LDAX_D] = LDAX_D,
            #endregion

            #region INX
            [OpcodeBytes.INX_B] = INX_B,
            [OpcodeBytes.INX_D] = INX_D,
            [OpcodeBytes.INX_H] = INX_H,
            [OpcodeBytes.INX_SP] = INX_SP,
            #endregion

            #region DCX
            [OpcodeBytes.DCX_B] = DCX_B,
            [OpcodeBytes.DCX_D] = DCX_D,
            [OpcodeBytes.DCX_H] = DCX_H,
            [OpcodeBytes.DCX_SP] = DCX_SP,
            #endregion

            #region PUSH
            [OpcodeBytes.PUSH_B] = PUSH_B,
            [OpcodeBytes.PUSH_D] = PUSH_D,
            [OpcodeBytes.PUSH_H] = PUSH_H,
            [OpcodeBytes.PUSH_PSW] = PUSH_PSW,
            #endregion

            #region POP
            [OpcodeBytes.POP_B] = POP_B,
            [OpcodeBytes.POP_D] = POP_D,
            [OpcodeBytes.POP_H] = POP_H,
            [OpcodeBytes.POP_PSW] = POP_PSW,
            #endregion

            #region ADD
            [OpcodeBytes.ADD_B] = ADD_B,
            [OpcodeBytes.ADD_C] = ADD_C,
            [OpcodeBytes.ADD_D] = ADD_D,
            [OpcodeBytes.ADD_E] = ADD_E,
            [OpcodeBytes.ADD_H] = ADD_H,
            [OpcodeBytes.ADD_L] = ADD_L,
            [OpcodeBytes.ADD_M] = ADD_M,
            [OpcodeBytes.ADD_A] = ADD_A,
            #endregion

            #region SUB
            [OpcodeBytes.SUB_B] = SUB_B,
            [OpcodeBytes.SUB_C] = SUB_C,
            [OpcodeBytes.SUB_D] = SUB_D,
            [OpcodeBytes.SUB_E] = SUB_E,
            [OpcodeBytes.SUB_H] = SUB_H,
            [OpcodeBytes.SUB_L] = SUB_L,
            [OpcodeBytes.SUB_M] = SUB_M,
            [OpcodeBytes.SUB_A] = SUB_A,
            #endregion

            #region ANA
            [OpcodeBytes.ANA_B] = ANA_B,
            [OpcodeBytes.ANA_C] = ANA_C,
            [OpcodeBytes.ANA_D] = ANA_D,
            [OpcodeBytes.ANA_E] = ANA_E,
            [OpcodeBytes.ANA_H] = ANA_H,
            [OpcodeBytes.ANA_L] = ANA_L,
            [OpcodeBytes.ANA_M] = ANA_M,
            [OpcodeBytes.ANA_A] = ANA_A,
            #endregion

            #region ORA
            [OpcodeBytes.ORA_B] = ORA_B,
            [OpcodeBytes.ORA_C] = ORA_C,
            [OpcodeBytes.ORA_D] = ORA_D,
            [OpcodeBytes.ORA_E] = ORA_E,
            [OpcodeBytes.ORA_H] = ORA_H,
            [OpcodeBytes.ORA_L] = ORA_L,
            [OpcodeBytes.ORA_M] = ORA_M,
            [OpcodeBytes.ORA_A] = ORA_A,
            #endregion

            #region ADC
            [OpcodeBytes.ADC_B] = ADC_B,
            [OpcodeBytes.ADC_C] = ADC_C,
            [OpcodeBytes.ADC_D] = ADC_D,
            [OpcodeBytes.ADC_E] = ADC_E,
            [OpcodeBytes.ADC_H] = ADC_H,
            [OpcodeBytes.ADC_L] = ADC_L,
            [OpcodeBytes.ADC_M] = ADC_M,
            [OpcodeBytes.ADC_A] = ADC_A,
            #endregion

            #region SBB
            [OpcodeBytes.SBB_B] = SBB_B,
            [OpcodeBytes.SBB_C] = SBB_C,
            [OpcodeBytes.SBB_D] = SBB_D,
            [OpcodeBytes.SBB_E] = SBB_E,
            [OpcodeBytes.SBB_H] = SBB_H,
            [OpcodeBytes.SBB_L] = SBB_L,
            [OpcodeBytes.SBB_M] = SBB_M,
            [OpcodeBytes.SBB_A] = SBB_A,
            #endregion

            #region XRA
            [OpcodeBytes.XRA_B] = XRA_B,
            [OpcodeBytes.XRA_C] = XRA_C,
            [OpcodeBytes.XRA_D] = XRA_D,
            [OpcodeBytes.XRA_E] = XRA_E,
            [OpcodeBytes.XRA_H] = XRA_H,
            [OpcodeBytes.XRA_L] = XRA_L,
            [OpcodeBytes.XRA_M] = XRA_M,
            [OpcodeBytes.XRA_A] = XRA_A,
            #endregion

            #region CMP
            [OpcodeBytes.CMP_B] = CMP_B,
            [OpcodeBytes.CMP_C] = CMP_C,
            [OpcodeBytes.CMP_D] = CMP_D,
            [OpcodeBytes.CMP_E] = CMP_E,
            [OpcodeBytes.CMP_H] = CMP_H,
            [OpcodeBytes.CMP_L] = CMP_L,
            [OpcodeBytes.CMP_M] = CMP_M,
            [OpcodeBytes.CMP_A] = CMP_A,
            #endregion

            #region DAD
            [OpcodeBytes.DAD_B] = DAD_B,
            [OpcodeBytes.DAD_D] = DAD_D,
            [OpcodeBytes.DAD_H] = DAD_H,
            [OpcodeBytes.DAD_SP] = DAD_SP,
            #endregion

            #region INR
            [OpcodeBytes.INR_B] = INR_B,
            [OpcodeBytes.INR_C] = INR_C,
            [OpcodeBytes.INR_D] = INR_D,
            [OpcodeBytes.INR_E] = INR_E,
            [OpcodeBytes.INR_H] = INR_H,
            [OpcodeBytes.INR_L] = INR_L,
            [OpcodeBytes.INR_M] = INR_M,
            [OpcodeBytes.INR_A] = INR_A,
            #endregion

            #region DCR
            [OpcodeBytes.DCR_B] = DCR_B,
            [OpcodeBytes.DCR_C] = DCR_C,
            [OpcodeBytes.DCR_D] = DCR_D,
            [OpcodeBytes.DCR_E] = DCR_E,
            [OpcodeBytes.DCR_H] = DCR_H,
            [OpcodeBytes.DCR_L] = DCR_L,
            [OpcodeBytes.DCR_M] = DCR_M,
            [OpcodeBytes.DCR_A] = DCR_A,
            #endregion
        };
    }
}
