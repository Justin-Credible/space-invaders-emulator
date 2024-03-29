using System;
using Xunit;

namespace JustinCredible.Intel8080.Tests
{
    public class OpcodesTests : BaseTest
    {
        [Fact]
        public void TestLookupTableHasCorrectBytes()
        {
            foreach (var entry in Opcodes.Lookup)
            {
                var opcodeByteIndex = entry.Key;
                var opcodeByteDefined = entry.Value.Code;

                var description = String.Format("The byte defined as the index (0x{0:X2}) and the byte as defined in the Opcode instance (0x{1:X2}) should be the same (for Opcode {2}).",
                    opcodeByteIndex,
                    opcodeByteDefined,
                    entry.Value.Instruction);

                Assert.True(opcodeByteIndex == opcodeByteDefined, description);
            }
        }
    }
}
