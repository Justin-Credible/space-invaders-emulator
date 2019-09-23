
namespace JustinCredible.SIEmulator
{
    /**
     * Assemble instruction name to opcode map derived from:
     * http://www.emulator101.com/8080-by-opcode.html
     */
    public class OpcodeBytes
    {
        public const byte NOP = 0x00;
        public const byte HLT = 0x76;

        public const byte LXI_B = 0x01;

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
    }
}
