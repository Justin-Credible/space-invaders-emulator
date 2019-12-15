
using System;
using System.Threading;

namespace JustinCredible.SIEmulator
{
    /**
     * Dedicated Shift Hardware
     *
     * The 8080 instruction set does not include opcodes for shifting.
     * An 8-bit pixel image must be shifted into a 16-bit word for the desired bit-position on the screen.
     * Space Invaders adds a hardware shift register to help with the math.
     * 
     * http://computerarcheology.com/Arcade/SpaceInvaders/Hardware.html
     */
    public class ShiftRegister
    {
        private int _offset = 0;

        private UInt16 _value = 0;

        /**
         * Writes the value into the upper 8 bits of the register after shifting the orginal
         * upper 8 bits into the lower 8 bits. The original lower 8 bits will be lost.
         */
        public void Write(byte data)
        {
            var valueShifted = (_value >> 8) & 0x00FF;
            var upperBytes = ((UInt16)data) << 8;
            _value = (UInt16)(upperBytes | valueShifted);
        }

        /**
         * Reads a byte from the register's value at the currently set offset value.
         */
        public byte Read()
        {
            // Shift left by the currently set offset, so we can just mask off the result
            // from the upper 8 bits.
            var result = _value << _offset;
            result = (UInt16)(result & 0xFF00);

            // Shift the upper 8 bits into the lower 8 bits so we can return the byte.
            result = result >> 8;
            return (byte)result;
        }

        /**
         * Used to set the offset value that will be used when reading from the register.
         */
        public void SetOffset(int offset)
        {
            // Sanity check; should only be three bits (decimal values 0-7 here).
            if (offset > 7)
                throw new Exception("Unexpected value for ShiftRegister::read() expecting a 3-bit value (decimal values 0-7 only).");

            _offset = offset;
        }
    }
}
