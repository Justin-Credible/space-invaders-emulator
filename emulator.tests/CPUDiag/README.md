
## Inte 8080 CPU Diagnostics Tests

This directory contains a diagnostics program used by the `CPUIntegrationTest` unit test. This is an Intel 8080 program the exercises all of the opcodes.

`cpudiag.asm` taken from: http://www.emulator101.com/files/cpudiag.asm
`cpudiag.bin` assembled version taken from: http://www.emulator101.com/files/cpudiag.bin
`cpudiag-disassembly.txt` disassembly of `cpudiag.bin` with 256 bytes padding prepended to match where it is expected to be loaded into memory (0x100) so the addresses of the disassembly match the memory addresses; useful for debugging.
