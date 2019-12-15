using System;
using System.IO;
using System.Text;
using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    public class ShiftRegisterTests : BaseTest
    {
        [Fact]
        public void ValuesAreShifted()
        {
            var shiftRegister = new ShiftRegister();

            Assert.Equal(0, shiftRegister.Read());

            shiftRegister.Write(0xAF);

            Assert.Equal(0xAF, shiftRegister.Read());

            shiftRegister.Write(0x5E);

            Assert.Equal(0x5E, shiftRegister.Read());

            shiftRegister.Write(0x29);

            Assert.Equal(0x29, shiftRegister.Read());
        }

        [Theory]
        [InlineData(0x5E, 0xAF, 0, 0x5E)] // 0x5EAF   (01011110) 10101111    => 0x5E (94)
        [InlineData(0x5E, 0xAF, 1, 0xBD)] // 0x5EAF   0(1011110  1)0101111    => 0xBD (189)
        [InlineData(0x5E, 0xAF, 2, 0x7A)] // 0x5EAF   01(011110  10)101111    => 0x7A (122)
        [InlineData(0x5E, 0xAF, 3, 0xF5)] // 0x5EAF   010(11110  101)01111    => 0xF5 (245)
        [InlineData(0x5E, 0xAF, 4, 0xEA)] // 0x5EAF   0101(1110  1010)1111    => 0xEA (234)
        [InlineData(0x5E, 0xAF, 5, 0xD5)] // 0x5EAF   01011(110  10101)111    => 0xD5 (213)
        [InlineData(0x5E, 0xAF, 6, 0xAB)] // 0x5EAF   010111(10  101011)11    => 0xAB (171)
        [InlineData(0x5E, 0xAF, 7, 0x57)] // 0x5EAF   0101111(0  1010111)1    => 0x57 (87)
        public void OffsetIsUsedDuringRead(byte upperByte, byte lowerByte, int offset, byte expected)
        {
            var shiftRegister = new ShiftRegister();

            Assert.Equal(0, shiftRegister.Read());

            shiftRegister.Write(lowerByte);
            shiftRegister.Write(upperByte);
            shiftRegister.SetOffset(offset);

            Assert.Equal(expected, shiftRegister.Read());

            // Ensure multiple reads aren't mutating the internal value during the offset shift operation.
            Assert.Equal(expected, shiftRegister.Read());
        }
    }
}
