using System;
using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class ConditionFlagsTests : BaseTest
    {
        [Theory]
        [InlineData(false, false, false, false, false, 0b00000010)]
        [InlineData(true, false, false, false, false, 0b10000010)]
        [InlineData(true, true, false, false, false, 0b11000010)]
        [InlineData(true, true, true, false, false, 0b11010010)]
        [InlineData(true, true, true, true, false, 0b11010110)]
        [InlineData(true, true, true, true, true, 0b11010111)]
        [InlineData(false, true, true, true, true, 0b01010111)]
        [InlineData(false, false, true, true, true, 0b00010111)]
        [InlineData(false, false, false, true, true, 0b00000111)]
        [InlineData(false, false, false, false, true, 0b00000011)]

        public void TestToByte(bool sign, bool zero, bool auxCarry, bool parity, bool carry, byte expected)
        {
            var flags = new ConditionFlags()
            {
                Sign = sign,
                Zero = zero,
                AuxCarry = auxCarry,
                Parity = parity,
                Carry = carry,
            };

            var actual = flags.ToByte();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(false, false, false, false, false, 0b00000010)]
        [InlineData(true, false, false, false, false, 0b10000010)]
        [InlineData(true, true, false, false, false, 0b11000010)]
        [InlineData(true, true, true, false, false, 0b11010010)]
        [InlineData(true, true, true, true, false, 0b11010110)]
        [InlineData(true, true, true, true, true, 0b11010111)]
        [InlineData(false, true, true, true, true, 0b01010111)]
        [InlineData(false, false, true, true, true, 0b00010111)]
        [InlineData(false, false, false, true, true, 0b00000111)]
        [InlineData(false, false, false, false, true, 0b00000011)]
        public void TestSetFromByte(bool expectedSign, bool expectedZero, bool expectedAuxCarry, bool expectedParity, bool expectedCarry, byte flagsAsByte)
        {
            var flags = new ConditionFlags();
            flags.SetFromByte(flagsAsByte);

            Assert.Equal(expectedSign, flags.Sign);
            Assert.Equal(expectedZero, flags.Zero);
            Assert.Equal(expectedAuxCarry, flags.AuxCarry);
            Assert.Equal(expectedParity, flags.Parity);
            Assert.Equal(expectedCarry, flags.Carry);
        }
    }
}
