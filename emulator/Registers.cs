
using System;

namespace JustinCredible.SIEmulator
{
    public struct Registers
    {
        /** Accumulator */
        public byte A;

        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte H;
        public byte L;

        public byte this[RegisterID registerID]
        {
            get
            {
                switch (registerID)
                {
                    case RegisterID.A:
                        return A;
                    case RegisterID.B:
                        return B;
                    case RegisterID.C:
                        return C;
                    case RegisterID.D:
                        return D;
                    case RegisterID.E:
                        return E;
                    case RegisterID.H:
                        return H;
                    case RegisterID.L:
                        return L;
                    default:
                        throw new NotImplementedException("Unandled register ID.");
                }
            }
            set
            {
                switch (registerID)
                {
                    case RegisterID.A:
                        A = value;
                        break;
                    case RegisterID.B:
                        B = value;
                        break;
                    case RegisterID.C:
                        C = value;
                        break;
                    case RegisterID.D:
                        D = value;
                        break;
                    case RegisterID.E:
                        E = value;
                        break;
                    case RegisterID.H:
                        H = value;
                        break;
                    case RegisterID.L:
                        L = value;
                        break;
                    default:
                        throw new NotImplementedException("Unandled register ID.");
                }
            }
        }
    }
}
