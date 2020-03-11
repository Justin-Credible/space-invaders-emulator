# Space Invaders Emulator

A work in progress implementation of an emulator for Space Invaders and the Intel 8080 CPU for fun and learning.

I started using the codebase from my [CHIP-8 emulator](https://github.com/Justin-Credible/CHIP-8-Emulator).

- [X] Intel 8080 CPU
  - [X] Opcodes
  - [X] Interrupts
  - [X] I/O
  - [X] Unit Tests (600+!)
  - [X] Integration Test (cpudiag.bin) 🎉
- [X] Space Invaders specific hardware
  - [X] CPU loop
  - [X] Interrupt generator
  - [X] I/O ports
  - [X] Shift register port
  - [X] Audio
  - [X] Framebuffer
- [ ] GUI / "Glue" Program
  - [X] CLI arguments parser
  - [X] Read ROM files
  - [X] SDL loop
  - [X] Read input keys
  - [ ] Play audio
  - [X] Render framebuffer
- [X] Debugging Tools
  - [X] Break during gameplay
  - [X] Break on specified addresses
  - [X] Interactive debugger (step/continue)
  - [X] Rewind / Reverse Step
  - [X] Save/Load State
  - [X] Disassembly with annotations
- [ ] Cleanup
  - [ ] Fix/update comments
  - [ ] Fix comment blocks to use C# style
  - [ ] Proper readme
