using System;
using System.Collections.Generic;

namespace JustinCredible.SIEmulator
{
    public class CPU
    {
        // Indicates the ROM has finished executing via a TODO opcode.
        // Step should not be called again without first calling Reset.
        public bool Finished { get; private set; }

        /**
         * ROM (8K)
         * $0000-$07ff:  invaders.h
         * $0800-$0fff:  invaders.g
         * $1000-$17ff:  invaders.f
         * $1800-$1fff:  invaders.e
         * 
         * RAM (8K)
         * $2000-$23ff:  work RAM (1K)
         * $2400-$3fff:  video RAM (7K)
         * 
         * $4000-:       RAM mirror
         */
        private byte[] _memory;

        // Registers
        private Registers _registers;

        // Program Counter; 16-bits
        private UInt16 _programCounter;

        // Stack Pointer; 16-bits
        private UInt16 _stackPointer;

        public CPU()
        {
            this.Reset();
        }

        public void Reset(int seed = -1)
        {
            // Initialize the regisgters and memory.
            _memory = new byte[16*1024];

            // The ROMs are loaded at the lower 8K of addressable memory.
            _programCounter = 0x0000;

            // Initialize the stack pointer.
            _stackPointer = 0x0000; // TODO: ???

            // Reset the flag that indicates that the ROM has finished executing.
            Finished = false;
        }

        private void LoadMemory(byte[] memory)
        {
            _memory = memory;
        }

        public void LoadRom(byte[] rom)
        {
            // Ensure the ROM data is not larger than we can load.
            if (rom.Length > 8192)
                throw new Exception("ROM filesize cannot exceed 8 kilobytes.");

            var memory = new byte[16*1024];

            // The ROM is the lower 8K of addressable memory.
            Array.Copy(rom, memory, 8192);

            LoadMemory(memory);
        }

        public CPUState DumpState()
        {
            return new CPUState()
            {
                Memory = _memory,
                Registers = _registers,
                ProgramCounter = _programCounter,
                StackPointer = _stackPointer,
            };
        }

        public void PrintDebugSummary()
        {
            var opcode = String.Format("0x{0:X2}", _memory[_programCounter]);
            var pc = String.Format("0x{0:X4}", _programCounter);
            var sp = String.Format("0x{0:X4}", _stackPointer);
            var regA = String.Format("0x{0:X2}", _registers.A);
            var regB = String.Format("0x{0:X2}", _registers.B);
            var regC = String.Format("0x{0:X2}", _registers.C);
            var regD = String.Format("0x{0:X2}", _registers.D);
            var regE = String.Format("0x{0:X2}", _registers.E);
            var regH = String.Format("0x{0:X2}", _registers.H);
            var regL = String.Format("0x{0:X2}", _registers.L);
            var regFlags = String.Format("0x{0:X2}", _registers.Flags);

            Console.WriteLine($"PC: ${pc}\tSP: ${sp}");
            Console.WriteLine($"A: ${regA}\tB: ${regB}\tC: ${regC}\tD: ${regD}");
            Console.WriteLine($"E: ${regE}\tH: ${regH}\tL: ${regL}");
            Console.WriteLine($"\tFlags: ${regFlags}");
        }

        public void Step(double elapsedMilliseconds, Dictionary<byte, bool> keys = null)
        {
            // Sanity check.
            if (Finished)
                throw new Exception("Program has finished execution; Reset() must be invoked before invoking Step() again.");

            // Fetch the next opcode to be executed.
            var opcode = _memory[_programCounter];

            // Useful for adding to IDE's watched variables during debugging.
            // var d_opcode = String.Format("0x{0:X2}", opcode);
            // var d_pc = String.Format("0x{0:X4}", _programCounter);
            // var d_sp = String.Format("0x{0:X4}", _stackPointer);
            // var d_A = String.Format("0x{0:X2}", _registers.A);
            // var d_B = String.Format("0x{0:X2}", _registers.B);
            // var d_C = String.Format("0x{0:X2}", _registers.C);
            // var d_D = String.Format("0x{0:X2}", _registers.D);
            // var d_E = String.Format("0x{0:X2}", _registers.E);
            // var d_H = String.Format("0x{0:X2}", _registers.H);
            // var d_L = String.Format("0x{0:X2}", _registers.L);
            // var d_Flags = String.Format("0x{0:X2}", _registers.Flags);

            // Decode and execute opcode.
            // There are 30 opcodes; each is two bytes and stored big-endian.
            if (opcode == 0x00)
            {

            }
            else
            {
                throw new NotImplementedException(String.Format("Attempted to execute unknown opcode 0x{0:X2} at memory address 0x{0:X4}", opcode, _programCounter));
            }
        }
    }
}
