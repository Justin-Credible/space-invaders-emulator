
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace JustinCredible.SIEmulator.Tests
{
    // NOTE: To prevent a ton of refactoring this is returning an instance
    // of the CPUConfig from the unit test Tests namesapce that includes
    // defaults that all of the unit tests use.
    public class CPUConfig : JustinCredible.SIEmulator.CPUConfig
    {
        public CPUConfig()
        {
            MemorySize = 16 * 1024;
            WriteableMemoryStart = 0x2000;
            WriteableMemoryEnd = 0x3FFFF;
            EnableDiagnosticsMode = false;
        }
    }
}
