
using System;

namespace JustinCredible.SIEmulator
{
    /**
     * Represents the collection registers available on the Intel 8080 CPU.
     */
    public class CPURegisters
    {
        /** Accumulator */
        public byte A;

        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte H;
        public byte L;

        public byte this[Register registerID]
        {
            get
            {
                switch (registerID)
                {
                    case Register.A:
                        return A;
                    case Register.B:
                        return B;
                    case Register.C:
                        return C;
                    case Register.D:
                        return D;
                    case Register.E:
                        return E;
                    case Register.H:
                        return H;
                    case Register.L:
                        return L;
                    default:
                        throw new NotImplementedException("Unandled register ID.");
                }
            }
            set
            {
                switch (registerID)
                {
                    case Register.A:
                        A = value;
                        break;
                    case Register.B:
                        B = value;
                        break;
                    case Register.C:
                        C = value;
                        break;
                    case Register.D:
                        D = value;
                        break;
                    case Register.E:
                        E = value;
                        break;
                    case Register.H:
                        H = value;
                        break;
                    case Register.L:
                        L = value;
                        break;
                    default:
                        throw new NotImplementedException("Unandled register ID.");
                }
            }
        }
    }
}
