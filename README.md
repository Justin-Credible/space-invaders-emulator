# Space Invaders Emulator

A work in progress implementation of an emulator for Space Invaders and the Intel 8080 CPU for fun and learning.

I started using the codebase from my [CHIP-8 emulator](https://github.com/Justin-Credible/CHIP-8-Emulator).

- [X] Intel 8080 CPU
  - [X] Opcodes
  - [X] Interrupts
  - [X] I/O
  - [X] Unit Tests (600+!)
  - [X] Integration Test (cpudiag.bin) 🎉
- [ ] Space Invaders specific hardware
  - [X] CPU loop
  - [X] Interrupt generator
  - [ ] I/O ports
  - [X] Shift register port
  - [ ] Audio
  - [ ] Framebuffer
- [ ] GUI / "Glue" Program
  - [X] CLI arguments parser
  - [ ] Read ROM files
  - [ ] SDL loop
  - [ ] Read input keys
  - [ ] Play audio
  - [ ] Render framebuffer
