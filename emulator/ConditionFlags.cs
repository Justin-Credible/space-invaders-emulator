
namespace JustinCredible.SIEmulator
{
    public struct ConditionFlags
    {
        /**
         * Z (zero) set to 1 when the result is equal to zero
         */
        public bool Zero;

        /**
         * S (sign) set to 1 when bit 7 (the most significant bit or MSB) of the math instruction is set
         */
        public bool Sign;

        /**
         * P (parity) is set when the answer has even parity, clear when odd parity
         */
        public bool Parity;

        /**
         * CY (carry) set to 1 when the instruction resulted in a carry out or borrow into the high order bit
         */
        public bool Carry;

        /**
         * AC (auxillary carry) is used mostly for BCD (binary coded decimal) math. Read the data book for more details, Space Invaders doesn't use it.
         */
        public bool AuxCarry;
    }
}
