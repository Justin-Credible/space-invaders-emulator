
namespace JustinCredible.SIEmulator
{
    /**
     * Assemble instruction name to opcode map derived from:
     * http://www.emulator101.com/8080-by-opcode.html
     */
    public class OpcodeBytes
    {
        public const byte NOP = 0x00;
        public const byte NOP2 = 0x10;
        public const byte NOP3 = 0x20;
        public const byte NOP4 = 0x30;
        public const byte NOP5 = 0x08;
        public const byte NOP6 = 0x18;
        public const byte NOP7 = 0x28;
        public const byte NOP8 = 0x38;

        public const byte HLT = 0x76;

        #region JUMP

        public const byte JMP = 0xc3;
        public const byte JMP2 = 0xcb;

        public const byte PCHL = 0xe9;
        public const byte JPO = 0xe2;
        public const byte JPE = 0xea;
        public const byte JP = 0xf2;
        public const byte JZ = 0xca;
        public const byte JNZ = 0xc2;
        public const byte JNC = 0xd2;
        public const byte JC = 0xda;
        public const byte JM = 0xfa;

        #endregion

        #region CALL

        public const byte CALL = 0xcd;
        public const byte CALL2 = 0xdd;
        public const byte CALL3 = 0xed;
        public const byte CALL4 = 0xfd;

        #endregion

        public const byte RET = 0xc9;
        public const byte RET2 = 0xd9;

        public const byte STA = 0x32;
        public const byte LDA = 0x3a;

        public const byte STC = 0x37;
        public const byte CMC = 0x3f;
        public const byte CMA = 0x2f;

        public const byte SHLD = 0x22;
        public const byte LHLD = 0x2a;

        public const byte RLC = 0x07;
        public const byte RRC = 0x0f;
        public const byte RAL = 0x17;
        public const byte RAR = 0x1f;

        #region MOV
        public const byte MOV_B_B = 0x40;
        public const byte MOV_B_C = 0x41;
        public const byte MOV_B_D = 0x42;
        public const byte MOV_B_E = 0x43;
        public const byte MOV_B_H = 0x44;
        public const byte MOV_B_L = 0x45;
        public const byte MOV_B_M = 0x46;
        public const byte MOV_B_A = 0x47;
        public const byte MOV_C_B = 0x48;
        public const byte MOV_C_C = 0x49;
        public const byte MOV_C_D = 0x4a;
        public const byte MOV_C_E = 0x4b;
        public const byte MOV_C_H = 0x4c;
        public const byte MOV_C_L = 0x4d;
        public const byte MOV_C_M = 0x4e;
        public const byte MOV_C_A = 0x4f;
        public const byte MOV_D_B = 0x50;
        public const byte MOV_D_C = 0x51;
        public const byte MOV_D_D = 0x52;
        public const byte MOV_D_E = 0x53;
        public const byte MOV_D_H = 0x54;
        public const byte MOV_D_L = 0x55;
        public const byte MOV_D_M = 0x56;
        public const byte MOV_D_A = 0x57;
        public const byte MOV_E_B = 0x58;
        public const byte MOV_E_C = 0x59;
        public const byte MOV_E_D = 0x5a;
        public const byte MOV_E_E = 0x5b;
        public const byte MOV_E_H = 0x5c;
        public const byte MOV_E_L = 0x5d;
        public const byte MOV_E_M = 0x5e;
        public const byte MOV_E_A = 0x5f;
        public const byte MOV_H_B = 0x60;
        public const byte MOV_H_C = 0x61;
        public const byte MOV_H_D = 0x62;
        public const byte MOV_H_E = 0x63;
        public const byte MOV_H_H = 0x64;
        public const byte MOV_H_L = 0x65;
        public const byte MOV_H_M = 0x66;
        public const byte MOV_H_A = 0x67;
        public const byte MOV_L_B = 0x68;
        public const byte MOV_L_C = 0x69;
        public const byte MOV_L_D = 0x6a;
        public const byte MOV_L_E = 0x6b;
        public const byte MOV_L_H = 0x6c;
        public const byte MOV_L_L = 0x6d;
        public const byte MOV_L_M = 0x6e;
        public const byte MOV_L_A = 0x6f;
        public const byte MOV_M_B = 0x70;
        public const byte MOV_M_C = 0x71;
        public const byte MOV_M_D = 0x72;
        public const byte MOV_M_E = 0x73;
        public const byte MOV_M_H = 0x74;
        public const byte MOV_M_L = 0x75;
        public const byte MOV_M_A = 0x77;
        public const byte MOV_A_B = 0x78;
        public const byte MOV_A_C = 0x79;
        public const byte MOV_A_D = 0x7a;
        public const byte MOV_A_E = 0x7b;
        public const byte MOV_A_H = 0x7c;
        public const byte MOV_A_L = 0x7d;
        public const byte MOV_A_M = 0x7e;
        public const byte MOV_A_A = 0x7f;
        #endregion MOV

        #region MVI
        public const byte MVI_B = 0x06;
        public const byte MVI_C = 0x0e;
        public const byte MVI_D = 0x16;
        public const byte MVI_E = 0x1e;
        public const byte MVI_H = 0x26;
        public const byte MVI_L = 0x2e;
        public const byte MVI_M = 0x36;
        public const byte MVI_A = 0x3e;
        #endregion

        #region LXI
        public const byte LXI_B = 0x01;
        public const byte LXI_D = 0x11;
        public const byte LXI_H = 0x21;
        public const byte LXI_SP = 0x31;
        #endregion

        #region STAX
        public const byte STAX_B = 0x02;
        public const byte STAX_D = 0x12;
        #endregion

        #region LDAX
        public const byte LDAX_B = 0x0a;
        public const byte LDAX_D = 0x1a;
        #endregion

        #region INX
        public const byte INX_B = 0x03;
        public const byte INX_D = 0x13;
        public const byte INX_H = 0x23;
        public const byte INX_SP = 0x33;
        #endregion

        #region DCX
        public const byte DCX_B = 0x0b;
        public const byte DCX_D = 0x1b;
        public const byte DCX_H = 0x2b;
        public const byte DCX_SP = 0x3b;
        #endregion

        #region PUSH
        public const byte PUSH_B = 0xc5;
        public const byte PUSH_D = 0xd5;
        public const byte PUSH_H = 0xe5;
        public const byte PUSH_PSW = 0xf5;
        #endregion

        #region POP
        public const byte POP_B = 0xc1;
        public const byte POP_D = 0xd1;
        public const byte POP_H = 0xe1;
        public const byte POP_PSW = 0xf1;
        #endregion

        #region ADD
        public const byte ADD_B = 0x80;
        public const byte ADD_C = 0x81;
        public const byte ADD_D = 0x82;
        public const byte ADD_E = 0x83;
        public const byte ADD_H = 0x84;
        public const byte ADD_L = 0x85;
        public const byte ADD_M = 0x86;
        public const byte ADD_A = 0x87;
        #endregion

        #region SUB
        public const byte SUB_B = 0x90;
        public const byte SUB_C = 0x91;
        public const byte SUB_D = 0x92;
        public const byte SUB_E = 0x93;
        public const byte SUB_H = 0x94;
        public const byte SUB_L = 0x95;
        public const byte SUB_M = 0x96;
        public const byte SUB_A = 0x97;
        #endregion

        #region ANA
        public const byte ANA_B = 0xa0;
        public const byte ANA_C = 0xa1;
        public const byte ANA_D = 0xa2;
        public const byte ANA_E = 0xa3;
        public const byte ANA_H = 0xa4;
        public const byte ANA_L = 0xa5;
        public const byte ANA_M = 0xa6;
        public const byte ANA_A = 0xa7;
        #endregion

        #region ORA
        public const byte ORA_B = 0xb0;
        public const byte ORA_C = 0xb1;
        public const byte ORA_D = 0xb2;
        public const byte ORA_E = 0xb3;
        public const byte ORA_H = 0xb4;
        public const byte ORA_L = 0xb5;
        public const byte ORA_M = 0xb6;
        public const byte ORA_A = 0xb7;
        #endregion

        #region ADC
        public const byte ADC_B = 0x88;
        public const byte ADC_C = 0x89;
        public const byte ADC_D = 0x8a;
        public const byte ADC_E = 0x8b;
        public const byte ADC_H = 0x8c;
        public const byte ADC_L = 0x8d;
        public const byte ADC_M = 0x8e;
        public const byte ADC_A = 0x8f;
        #endregion

        #region SBB
        public const byte SBB_B = 0x98;
        public const byte SBB_C = 0x99;
        public const byte SBB_D = 0x9a;
        public const byte SBB_E = 0x9b;
        public const byte SBB_H = 0x9c;
        public const byte SBB_L = 0x9d;
        public const byte SBB_M = 0x9e;
        public const byte SBB_A = 0x9f;
        #endregion

        #region XRA
        public const byte XRA_B = 0xa8;
        public const byte XRA_C = 0xa9;
        public const byte XRA_D = 0xaa;
        public const byte XRA_E = 0xab;
        public const byte XRA_H = 0xac;
        public const byte XRA_L = 0xad;
        public const byte XRA_M = 0xae;
        public const byte XRA_A = 0xaf;
        #endregion

        #region CMP
        public const byte CMP_B = 0xb8;
        public const byte CMP_C = 0xb9;
        public const byte CMP_D = 0xba;
        public const byte CMP_E = 0xbb;
        public const byte CMP_H = 0xbc;
        public const byte CMP_L = 0xbd;
        public const byte CMP_M = 0xbe;
        public const byte CMP_A = 0xbf;
        #endregion

        #region DAD
        public const byte DAD_B = 0x09;
        public const byte DAD_D = 0x19;
        public const byte DAD_H = 0x29;
        public const byte DAD_SP = 0x39;
        #endregion

        #region INR
        public const byte INR_B = 0x04;
        public const byte INR_C = 0x0c;
        public const byte INR_D = 0x14;
        public const byte INR_E = 0x1c;
        public const byte INR_H = 0x24;
        public const byte INR_L = 0x2c;
        public const byte INR_M = 0x34;
        public const byte INR_A = 0x3c;
        #endregion

        #region DCR
        public const byte DCR_B = 0x05;
        public const byte DCR_C = 0x0d;
        public const byte DCR_D = 0x15;
        public const byte DCR_E = 0x1d;
        public const byte DCR_H = 0x25;
        public const byte DCR_L = 0x2d;
        public const byte DCR_M = 0x35;
        public const byte DCR_A = 0x3d;
        #endregion
    }
}
