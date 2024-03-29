
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using JustinCredible.I8080Disassembler;
using JustinCredible.Intel8080;

namespace JustinCredible.SIEmulator
{
    /**
     * An implementation of the Space Invaders hardware for emulation; this includes the
     * Intel 8080 CPU instance, shift hardware, interrupts, debugger, and hardware loop.
     */
    public class SpaceInvaders
    {
        #region Constants

        // The Intel 8080 for Space Invadiers is clocked at 2MHz.
        private const int CPU_MHZ = 2000000;

        // While the resolution is indeed 256x224, note that the monitor for this
        // game is portrait, not landscape. It is rotated -90 degrees (counterclockwise)
        // in the cabinet and therefore the resolution viewable to the user will be 224x256.
        // The framebuffer will need to be rotated before it is displayed to the end user.
        public const int RESOLUTION_WIDTH = 256;
        public const int RESOLUTION_HEIGHT = 224;

        // The frame buffer is 256 x 224, which is 57,344 pixels. Since the display is only
        // black and white, we only need one bit per pixel. Therefore we need 57,344 / 8
        // => 7,168 bytes or 7 KB for the frame buffer. This is pulled from the video RAM
        // portion which is at $2400-$3fff.
        private const int FRAME_BUFFER_SIZE = 1024 * 7;

        #endregion

        #region Events

        // Fired when a frame is ready to be rendered.
        public delegate void RenderEvent(RenderEventArgs e);
        public event RenderEvent OnRender;
        private RenderEventArgs _renderEventArgs;

        // Fired when a sound effect should be played.
        public delegate void SoundEvent(SoundEventArgs e);
        public event SoundEvent OnSound;
        private SoundEventArgs _soundEventArgs;

        #endregion

        #region Hardware

        // The thread on which we'll run the hardware emulation loop.
        private Thread _thread;

        // Indicates if a stop was requested via the Stop() method. Used to break out of the hardware
        // loop in the thread and stop execution.
        private bool _cancelled = false;

        // The configuration of the Intel 8080 CPU specifically for Space Invaders.
        private static readonly CPUConfig _cpuConfig = new CPUConfig()
        {
            // 16 KB of RAM
            MemorySize = 16 * 1024,

            /**
             * This is the memory layout specific to Space Invaders:
             * 
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
             * RAM Mirror (8K)
             * $4000-$43ff:  work RAM (1K)
             * $4400-$5fff:  video RAM (7K)
             */
            WriteableMemoryStart = 0x2000,
            WriteableMemoryEnd = 0x3FFF,
            MirrorMemoryStart = 0x4000,
            MirrorMemoryEnd = 0x5FFF,

            // Interrupts are initially disabled, and will be enabled by the program ROM when ready.
            InterruptsEnabled = false,

            EnableDiagnosticsMode = false,
        };

        // Intel 8080
        private CPU _cpu;

        // Dedicated Shift Hardware
        private ShiftRegister _shiftRegister;

        // The game's video hardware generates runs at 60hz. It generates two interrupts @ 60hz. Interrupt
        // #1 the middle of a frame and interrupt #2 at the end (vblank). To simulate this, we'll calculate
        // the number of cycles we're expecting between each of these interrupts. While this is not entirely
        // accurate, it is close enough for the game to run as expected.
        private double _cyclesPerInterrupt = Math.Floor(Convert.ToDouble(CPU_MHZ / 60 / 2));
        private int _cyclesSinceLastInterrupt = 0;
        private Interrupt _nextInterrupt;

        // To keep the emulated CPU from running too fast, we use a stopwatch and count cycles.
        private Stopwatch _cpuStopWatch = new Stopwatch();
        private int _cycleCount = 0;

        // Holds the last data written by the CPU to ports 3 and 5, which are used for sound effects.
        // The CPU holds the bits high when the sounds are playing, and then flips low to stop.
        // For our purposes, we just need to know when the bits flip so we can emit a sound effect
        // event once for a given sound device write.
        private byte _device3WriteLastData = 0x00;
        private byte _device5WriteLastData = 0x00;

        #endregion

        #region Dip Switches

        public StartingShipsSetting StartingShips { get; set; } = StartingShipsSetting.Three;
        public ExtraShipAtSetting ExtraShipAt { get; set; } = ExtraShipAtSetting.Points1000;

        #endregion

        #region Button State

        public bool ButtonP1Left { get; set; } = false;
        public bool ButtonP1Right { get; set; } = false;
        public bool ButtonP1Fire { get; set; } = false;
        public bool ButtonP2Left { get; set; } = false;
        public bool ButtonP2Right { get; set; } = false;
        public bool ButtonP2Fire { get; set; } = false;
        public bool ButtonStart1P { get; set; } = false;
        public bool ButtonStart2P { get; set; } = false;
        public bool ButtonCredit { get; set; } = false;
        public bool ButtonTilt { get; set; } = false;

        #endregion

        #region Debugging Features

        private static readonly int MAX_ADDRESS_HISTORY = 100;
        private static readonly int MAX_REWIND_HISTORY = 20;

        private int _totalCycles = 0;
        private int _totalSteps = 0;

        /**
         * Enables debugging statistics and features.
         */
        public bool Debug { get; set; } = false;

        /**
         * When Debug=true, stores the last MAX_ADDRESS_HISTORY values of the program counter.
         */
        private List<UInt16> _addressHistory = new List<UInt16>();

        /**
         * When Debug=true, the program will break at these addresses and allow the user to perform
         * interactive debugging via the console.
         */
        public List<UInt16> BreakAtAddresses { get; set; }

        /**
         * When Debug=true, allows for single reverse-stepping in the interactive debugging console.
         */
        public bool RewindEnabled { get; set; } = false;

        /**
         * When Debug=true and RewindEnabled=true, stores sufficient state of the CPU and emulator
         * to allow for stepping backwards to each state of the system.
         */
        private List<EmulatorState> _executionHistory = new List<EmulatorState>();

        /**
         * Indicates if we're stingle stepping through opcodes/instructions using the interactive
         * debugger when Debug=true.
         */
        private bool _singleStepping = false;

        /**
         * For use by the interactive debugger when Debug=true. If true, indicates that the disassembly
         * should be annotated with the values in the Annotations dictionary. If false, the diassembler
         * will annotate each line with a pseudocode comment instead.
         */
        private bool _showAnnotatedDisassembly = false;

        /**
         * The annotations to be used when Debug=true and _showAnnotatedDisassembly=true. It is a map
         * of memory addresses to string annotation values.
         */
        public Dictionary<UInt16, String> Annotations { get; set; }

        #endregion

        #region Public Methods

        /**
         * Used to start execution of the CPU with the given ROM and optional emulator state.
         * The emulator's hardware loop will run on a spereate thread, and therefore, this method
         * is non-blocking.
         */
        public void Start(byte[] rom, EmulatorState state = null)
        {
            if (_thread != null)
                throw new Exception("Emulator cannot be started because it was already running.");

            _cyclesSinceLastInterrupt = 0;
            _nextInterrupt = Interrupt.One;

            _cpu = new CPU(_cpuConfig);

            _cpu.OnDeviceRead += CPU_OnDeviceRead;
            _cpu.OnDeviceWrite += CPU_OnDeviceWrite;

            // Map the ROM into the lower 8K of the memory space.
            var memory = new byte[_cpuConfig.MemorySize];
            Array.Copy(rom, memory, rom.Length);

            _cpu.LoadMemory(memory);

            _shiftRegister = new ShiftRegister();

            _renderEventArgs = new RenderEventArgs()
            {
                ShouldRender = false,
                FrameBuffer = new byte[FRAME_BUFFER_SIZE],
            };

            _soundEventArgs = new SoundEventArgs();

            if (state != null)
                LoadState(state);

            _cancelled = false;
            _thread = new Thread(new ThreadStart(HardwareLoop));
            _thread.Name = "Emulator: Hardware Loop";
            _thread.Start();
        }

        /**
         * Used to stop execution of the CPU and halt the thread.
         */
        public void Stop()
        {
            if (_thread == null)
                throw new Exception("Emulator cannot be stopped because it wasn't running.");

            _cancelled = true;
        }

        /**
         * Used to stop CPU execution and enable single stepping through opcodes via the interactive
         * debugger (only when Debug = true).
         */
        public void Break()
        {
            if (Debug)
                _singleStepping = true;
        }

        #endregion

        #region CPU Event Handlers

        /**
         * Used to handle the CPU's IN instruction; read value from given device ID.
         */
        private byte CPU_OnDeviceRead(int deviceID)
        {
            // http://computerarcheology.com/Arcade/SpaceInvaders/Hardware.html
            switch (deviceID)
            {
                // Port 0
                //  bit 0 DIP4 (Seems to be self-test-request read at power up)
                //  bit 1 Always 1
                //  bit 2 Always 1
                //  bit 3 Always 1
                //  bit 4 Fire
                //  bit 5 Left
                //  bit 6 Right
                //  bit 7 ? tied to demux port 7 ?
                case 0x00:
                    return 0b01110000;

                // Port 1
                //  bit 0 = CREDIT (1 if deposit)
                //  bit 1 = 2P start (1 if pressed)
                //  bit 2 = 1P start (1 if pressed)
                //  bit 3 = Always 1
                //  bit 4 = 1P shot (1 if pressed)
                //  bit 5 = 1P left (1 if pressed)
                //  bit 6 = 1P right (1 if pressed)
                //  bit 7 = Not connected
                case 0x01:
                {
                    int value = 0b00001000;

                    if (ButtonCredit)
                        value = value | 0b00000001;

                    if (ButtonStart2P)
                        value = value | 0b00000010;

                    if (ButtonStart1P)
                        value = value | 0b00000100;

                    if (ButtonP1Fire)
                        value = value | 0b00010000;

                    if (ButtonP1Left)
                        value = value | 0b00100000;

                    if (ButtonP1Right)
                        value = value | 0b01000000;

                    return (byte)value;
                }

                // Port 2
                //  bit 0 = DIP3 00 = 3 ships  10 = 5 ships
                //  bit 1 = DIP5 01 = 4 ships  11 = 6 ships
                //  bit 2 = Tilt
                //  bit 3 = DIP6 0 = extra ship at 1500, 1 = extra ship at 1000
                //  bit 4 = P2 shot (1 if pressed)
                //  bit 5 = P2 left (1 if pressed)
                //  bit 6 = P2 right (1 if pressed)
                //  bit 7 = DIP7 Coin info displayed in demo screen 0=ON
                case 0x02:
                {
                    int value = 0b00000000;

                    switch (StartingShips)
                    {
                        case StartingShipsSetting.Six:
                            value = value | 0b00000011;
                            break;
                        case StartingShipsSetting.Five:
                            value = value | 0b00000010;
                            break;
                        case StartingShipsSetting.Four:
                            value = value | 0b00000001;
                            break;
                        case StartingShipsSetting.Three:
                        default:
                            break;
                    }

                    if (ButtonTilt)
                        value = value | 0b00000100;

                    if (ExtraShipAt == ExtraShipAtSetting.Points1000)
                        value = value | 0b00001000;

                    if (ButtonP2Fire)
                        value = value | 0b00010000;

                    if (ButtonP2Left)
                        value = value | 0b00100000;

                    if (ButtonP2Right)
                        value = value | 0b01000000;

                    return (byte)value;
                }

                // Shift Register - Read
                case 0x03:
                    return _shiftRegister.Read();

                default:
                    Console.WriteLine($"WARNING: An IN/Read for port {deviceID} is not implemented.");
                    return 0x00;
            }
        }

        /**
         * Used to handle the CPU's OUT instruction; write value to given device ID.
         */
        private void CPU_OnDeviceWrite(int deviceID, byte data)
        {
            //http://computerarcheology.com/Arcade/SpaceInvaders/Hardware.html
            switch (deviceID)
            {
                // Shift Register - Set Offset
                case 0x02:
                    _shiftRegister.SetOffset(data);
                    break;

                // Shift Register - Write
                case 0x04:
                    _shiftRegister.Write(data);
                    break;

                case 0x03: // Sounds
                    HandleSoundEffects(0x03, data);
                    break;

                case 0x05: // Sounds
                    HandleSoundEffects(0x05, data);
                    break;

                case 0x06: // Watchdog
                    break;

                default:
                    Console.WriteLine($"WARNING: An OUT/Write for port {deviceID} (value: {data}) is not implemented.");
                    break;
            }
        }

        #endregion

        #region Private Methods: Hardware Loop

        /**
         * Handles stepping the CPU to execute instructions as well as firing interrupts.
         */
        private void HardwareLoop()
        {
            _cpuStopWatch.Start();
            _cycleCount = 0;

            try
            {
                while (!_cancelled)
                {
                    // Handle all the debug tasks that need to happen before we execute an instruction.
                    if (Debug)
                        HandleDebugFeaturesPreStep();

                    // Step the CPU to execute the next instruction.
                    var cycles = _cpu.Step();

                    // Keep track of the number of cycles to see if we need to throttle the CPU.
                    _cycleCount += cycles;

                    // Handle all the debug tasks that need to happen after we execute an instruction.
                    if (Debug)
                        HandleDebugFeaturesPostStep(cycles);

                    // Throttle the CPU emulation if needed.
                    if (_cycleCount >= (CPU_MHZ/60))
                    {
                        _cpuStopWatch.Stop();

                        if (_cpuStopWatch.Elapsed.TotalMilliseconds < 16.6)
                        {
                            var sleepForMs = 16.6 - _cpuStopWatch.Elapsed.TotalMilliseconds;

                            if (sleepForMs >= 1)
                                System.Threading.Thread.Sleep((int)sleepForMs);
                        }

                        _cycleCount = 0;
                        _cpuStopWatch.Restart();
                    }

                    // See if it's time to fire a CPU interrupt or not.
                    HandleInterrupts(cycles);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("An exception occurred during emulation!");
                PrintDebugSummary(_showAnnotatedDisassembly);
                Console.WriteLine("-------------------------------------------------------------------");
                throw exception;
            }

            _cpu = null;
            _thread = null;
        }

        /**
         * Space Invaders sends two different interrupts, which are driven by the video hardware.
         * We can use the number of CPU cycles elapsed to roughtly estimate when these interrupts
         * should fire. If it's time for an interrupt, we disable interrupts and then jump to the
         * interupt handler. Once the handler completes, we re-enable interrupts and reset the
         * program counter back to it's original location so the CPU can continue where it left off.
         */
        private void HandleInterrupts(int cyclesElapsed)
        {
            // Keep track of the number of cycles since the last interrupt occurred.
            _cyclesSinceLastInterrupt += cyclesElapsed;

            // Determine if it's time for the video hardware to fire an interrupt.
            if (_cyclesSinceLastInterrupt < _cyclesPerInterrupt)
                return;

            // If interrupts are enabled, then handle them, otherwise do nothing.
            if (_cpu.InterruptsEnabled)
            {
                // If we're going to run an interrupt handler, ensure interrupts are disabled.
                // This ensures we don't interrupt the interrupt handler. The program ROM will
                // re-enable the interrupts manually.
                _cpu.InterruptsEnabled = false;

                // Execute the handler for the interrupt.
                _cpu.StepInterrupt(_nextInterrupt);

                // The video hardware alternates between these two interrupts.
                if (_nextInterrupt == Interrupt.One)
                {
                    // CRT electron beam is at the middle of the screen (approximately).

                    _nextInterrupt = Interrupt.Two;
                }
                else if (_nextInterrupt == Interrupt.Two)
                {
                    // CRT electron beam reached the end (V-Blank).

                    if (OnRender != null)
                    {
                        Array.Copy(_cpu.Memory, 0x2400, _renderEventArgs.FrameBuffer, 0, FRAME_BUFFER_SIZE);
                        OnRender(_renderEventArgs);
                    }

                    _nextInterrupt = Interrupt.One;
                }
                else
                    throw new Exception($"Unexpected next interrupt: {_nextInterrupt}.");
            }

            // Reset the count so we can count up again.
            _cyclesSinceLastInterrupt = 0;
        }

        /**
         * Handles playing sound effects given the device ID and data written by the CPU. This emits an
         * event with the name of the sound effect to be played. Normally, the bits are held at 1 for
         * the duration of the sound effect and then flip back to 0 when complete. This method handles
         * this case and only emits sound effect events if the bit flips from 0 to 1. It also handles
         * the special case for the UFO sound effect which repeats until the UFO disappears.
         */
        private void HandleSoundEffects(int deviceID, byte data)
        {
            if (OnSound == null)
                return;

            if (deviceID == 0x03) // Port 3
            {
                if ((_device3WriteLastData & 0b00000001) == 0 && (data & 0b00000001) == 0b00000001) // Bit 0 - UFO - Start repeating sound
                {
                    _soundEventArgs.SoundEffect = SoundEffect.UFO_Start;
                    OnSound(_soundEventArgs);
                }
                else if ((_device3WriteLastData & 0b00000001) == 1 && (data & 0b00000001) == 0b00000000) // Bit 0 - UFO - Stop repeating sound
                {
                    _soundEventArgs.SoundEffect = SoundEffect.UFO_Stop;
                    OnSound(_soundEventArgs);
                }

                if ((_device3WriteLastData & 0b00000010) == 0 && (data & 0b00000010) == 0b00000010) // Bit 1 - Shot
                {
                    _soundEventArgs.SoundEffect = SoundEffect.Shot;
                    OnSound(_soundEventArgs);
                }

                if ((_device3WriteLastData & 0b00000100) == 0 && (data & 0b00000100) == 0b00000100) // Bit 2 - Player Died
                {
                    _soundEventArgs.SoundEffect = SoundEffect.PlayerDied;
                    OnSound(_soundEventArgs);
                }

                if ((_device3WriteLastData & 0b00001000) == 0 && (data & 0b00001000) == 0b00001000) // Bit 3 - Invader Killed
                {
                    _soundEventArgs.SoundEffect = SoundEffect.InvaderKilled;
                    OnSound(_soundEventArgs);
                }

                _device3WriteLastData = data;
            }
            else if (deviceID == 0x05) // Port 5
            {
                if ((_device3WriteLastData & 0b00000001) == 0 && (data & 0b00000001) == 0b00000001) // Bit 0 - Invader Movement 1
                {
                    _soundEventArgs.SoundEffect = SoundEffect.InvaderMove1;
                    OnSound(_soundEventArgs);
                }

                if ((_device3WriteLastData & 0b00000010) == 0 && (data & 0b00000010) == 0b00000010) // Bit 1 - Invader Movement 2
                {
                    _soundEventArgs.SoundEffect = SoundEffect.InvaderMove2;
                    OnSound(_soundEventArgs);
                }

                if ((_device3WriteLastData & 0b00000100) == 0 && (data & 0b00000100) == 0b00000100) // Bit 2 - Invader Movement 3
                {
                    _soundEventArgs.SoundEffect = SoundEffect.InvaderMove3;
                    OnSound(_soundEventArgs);
                }

                if ((_device3WriteLastData & 0b00001000) == 0 && (data & 0b00001000) == 0b00001000) // Bit 3 - Invader Movement 4
                {
                    _soundEventArgs.SoundEffect = SoundEffect.InvaderMove4;
                    OnSound(_soundEventArgs);
                }

                if ((_device3WriteLastData & 0b00010000) == 0 && (data & 0b00010000) == 0b00010000) // Bit 4 - UFO Hit
                {
                    _soundEventArgs.SoundEffect = SoundEffect.UFOHit;
                    OnSound(_soundEventArgs);
                }

                _device5WriteLastData = data;
            }
            else
                throw new Exception("Unexpected device ID for sound effect: " + deviceID);
        }

        /**
         * This method handles all the work that needs to be done when debugging is enabled right before
         * the CPU executes an opcode. This includes recording CPU stats, address history, and CPU breakpoints,
         * as well as implements the interactive debugger.
         */
        private void HandleDebugFeaturesPreStep()
        {
            // Record the current address.

            _addressHistory.Add(_cpu.ProgramCounter);

            if (_addressHistory.Count >= MAX_ADDRESS_HISTORY)
                _addressHistory.RemoveAt(0);

            // See if we need to break based on a given address.
            if (BreakAtAddresses != null && BreakAtAddresses.Contains(_cpu.ProgramCounter))
                _singleStepping = true;

            // If we need to break, print out the CPU state and wait for a keypress.
            if (_singleStepping)
            {
                // Print debug information and wait for user input via the console (key press).
                while (true)
                {
                    Console.WriteLine("-------------------------------------------------------------------");
                    PrintDebugSummary(_showAnnotatedDisassembly);

                    var rewindPrompt = "";

                    if (RewindEnabled && _executionHistory.Count > 0)
                        rewindPrompt = "F9 = Step Backward    ";

                    Console.WriteLine($"  F1 = Save State    F2 = Load State    F4 = Edit Breakpoints");
                    Console.WriteLine($"  F5 = Continue    {rewindPrompt}F10 = Step");
                    Console.WriteLine("  F11 = Toggle Annotated Disassembly    F12 = Print Last 30 Opcodes");
                    var key = Console.ReadKey(); // Blocking

                    // Handle console input.
                    if (key.Key == ConsoleKey.F1) // Save State
                    {
                        var state = SaveState();
                        var json = JsonSerializer.Serialize<EmulatorState>(state);

                        Console.WriteLine(" Enter file name/path to write save state...");
                        var filename = Console.ReadLine();

                        File.WriteAllText(filename, json);

                        Console.WriteLine("  State Saved!");
                    }
                    else if (key.Key == ConsoleKey.F2) // Load State
                    {
                        Console.WriteLine(" Enter file name/path to read save state...");
                        var filename = Console.ReadLine();

                        var json = File.ReadAllText(filename);

                        var state = JsonSerializer.Deserialize<EmulatorState>(json);
                        LoadState(state);

                        Console.WriteLine("  State Loaded!");
                    }
                    else if (key.Key == ConsoleKey.F4) // Edit Breakpoints
                    {
                        if (BreakAtAddresses == null)
                            BreakAtAddresses = new List<ushort>();

                        while (true)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Current break point addressess:");

                            if (BreakAtAddresses.Count == 0)
                            {
                                Console.Write(" (none)");
                            }
                            else
                            {
                                foreach (var breakAtAddress in BreakAtAddresses)
                                    Console.WriteLine(String.Format(" • 0x{0:X4}", breakAtAddress));
                            }

                            Console.WriteLine("  Enter an address to toggle breakpoint (e.g. '0x1234<ENTER>') or leave empty and press <ENTER> to stop editing breakpoints...");
                            var addressString = Console.ReadLine();

                            if (String.IsNullOrWhiteSpace(addressString))
                                break; // Break out of input loop

                            var address = Convert.ToUInt16(addressString, 16);

                            if (BreakAtAddresses.Contains(address))
                                BreakAtAddresses.Remove(address);
                            else
                                BreakAtAddresses.Add(address);
                        }
                    }
                    else if (key.Key == ConsoleKey.F5) // Continue
                    {
                        _singleStepping = false;
                        break; // Break out of input loop
                    }
                    else if (RewindEnabled && _executionHistory.Count > 0 && key.Key == ConsoleKey.F9) // Step Backward
                    {
                        var state = _executionHistory[_executionHistory.Count - 1];
                        _executionHistory.RemoveAt(_executionHistory.Count - 1);

                        LoadState(state);
                        _cyclesSinceLastInterrupt -= state.LastCyclesExecuted.Value;
                    }
                    else if (key.Key == ConsoleKey.F10) // Step
                    {
                        break; // Break out of input loop
                    }
                    else if (key.Key == ConsoleKey.F11) // Toggle Annotated Disassembly
                    {
                        _showAnnotatedDisassembly = !_showAnnotatedDisassembly;
                    }
                    else if (key.Key == ConsoleKey.F12) // Print List 10 Opcodes
                    {
                        Console.WriteLine("-------------------------------------------------------------------");
                        Console.WriteLine();
                        Console.WriteLine(" Last 30 Opcodes: ");
                        Console.WriteLine();
                        PrintRecentInstructions(30);
                    }
                }
            }
        }

        /**
         * This method handles all the work that needs to be done when debugging is enabled right after
         * the CPU executes an opcode. This includes recording CPU stats and rewind history.
         */
        private void HandleDebugFeaturesPostStep(int cyclesElapsed)
        {
            // Keep track of the total number of steps (instructions) and cycles ellapsed.
            _totalSteps++;
            _totalCycles += cyclesElapsed;

            // Used to slow down the emulation to watch the renderer.
            // if (_totalCycles % 1000 == 0)
            //     System.Threading.Thread.Sleep(10);

            if (RewindEnabled)
            {
                if (_executionHistory.Count >= MAX_REWIND_HISTORY)
                    _executionHistory.RemoveAt(0);

                var state = SaveState();
                state.LastCyclesExecuted = cyclesElapsed;

                _executionHistory.Add(state);
            }
        }

        #endregion

        #region Private Methods: Save/Load State

        /**
         * Used to dump the state of the CPU and all fields needed to restore the emulator's
         * state in order to continue at this execution point later on.
         */
        private EmulatorState SaveState()
        {
            return new EmulatorState()
            {
                Registers = new CPURegisters()
                {
                    A = _cpu.Registers.A,
                    B = _cpu.Registers.B,
                    C = _cpu.Registers.C,
                    D = _cpu.Registers.D,
                    E = _cpu.Registers.E,
                    H = _cpu.Registers.H,
                    L = _cpu.Registers.L,
                },
                Flags = new ConditionFlags()
                {
                    Zero = _cpu.Flags.Zero,
                    Sign = _cpu.Flags.Sign,
                    Parity = _cpu.Flags.Parity,
                    Carry = _cpu.Flags.Carry,
                    AuxCarry = _cpu.Flags.AuxCarry,
                },
                ProgramCounter = _cpu.ProgramCounter,
                StackPointer = _cpu.StackPointer,
                InterruptsEnabled = _cpu.InterruptsEnabled,
                Memory = _cpu.Memory.Clone() as byte[],
                TotalCycles = _totalCycles,
                TotalSteps = _totalSteps,
                CyclesSinceLastInterrupt = _cyclesSinceLastInterrupt,
                NextInterrupt = _nextInterrupt,
            };
        }

        /**
         * Used to restore the state of the CPU and restore all fields to allow the emulator
         * to continue execution from a previously saved state.
         */
        private void LoadState(EmulatorState state)
        {
            _cpu.Registers = state.Registers;
            _cpu.Flags = state.Flags;
            _cpu.ProgramCounter = state.ProgramCounter;
            _cpu.StackPointer = state.StackPointer;
            _cpu.InterruptsEnabled = state.InterruptsEnabled;
            _cpu.Memory = state.Memory;
            _totalCycles = state.TotalCycles;
            _totalSteps = state.TotalSteps;
            _cyclesSinceLastInterrupt = state.CyclesSinceLastInterrupt;
            _nextInterrupt = state.NextInterrupt;
        }

        #endregion

        #region Private Methods: Debugging & Diagnostics

        private void PrintDebugSummary(bool showAnnotatedDisassembly = false)
        {
            Console.WriteLine("-------------------------------------------------------------------");

            if (Debug)
                Console.WriteLine($" Total Steps/Opcodes: {_totalSteps}\tCPU Cycles: {_totalCycles}");

            Console.WriteLine();
            _cpu.PrintDebugSummary();
            Console.WriteLine();
            PrintCurrentExecution(showAnnotatedDisassembly);
            Console.WriteLine();
        }

        /**
         * Prints last n instructions that were executed up to MAX_ADDRESS_HISTORY.
         * Useful when a debugger is attached. Only works when Debug is true.
         */
        private void PrintRecentInstructions(int count = 10)
        {
            if (!Debug)
                return;

            var output = new StringBuilder();

            if (count > _addressHistory.Count)
                count = _addressHistory.Count;

            var startIndex = _addressHistory.Count - count;

            for (var i = startIndex; i < _addressHistory.Count; i++)
            {
                var address = _addressHistory[i];

                // Edge case for being able to print instruction history when we've jumped outside
                // of the allowable memory locations.
                if (address >= _cpu.Memory.Length)
                {
                    var addressDisplay = String.Format("0x{0:X4}", address);
                    output.AppendLine($"[IndexOutOfRange: {addressDisplay}]");
                    continue;
                }

                var instruction = Disassembler.Disassemble(_cpu.Memory, address, out _, true, true);
                output.AppendLine(instruction);
            }

            Console.WriteLine(output.ToString());
        }

        /**
         * Used to print the disassembly of memory locations before and after the given address.
         * Useful when a debugger is attached.
         */
        private void PrintMemory(UInt16 address, bool annotate = false, int beforeCount = 10, int afterCount = 10)
        {
            var output = new StringBuilder();

            // Ensure the start and end locations are within range.
            var start = (address - beforeCount < 0) ? 0 : (address - beforeCount);
            var end = (address + afterCount >= _cpu.Memory.Length) ? _cpu.Memory.Length - 1 : (address + afterCount);

            for (var i = start; i < end; i++)
            {
                var addressIndex = (UInt16)i;

                // If this is the current address location, add an arrow pointing to it.
                output.Append(address == addressIndex ? "---->\t" : "\t");

                // If we're showing annotations, then don't show the pseudocode.
                var emitPseudocode = !_showAnnotatedDisassembly;

                // Disasemble the opcode and print it.
                var instruction = Disassembler.Disassemble(_cpu.Memory, addressIndex, out int instructionLength, true, emitPseudocode);
                output.Append(instruction);

                // If we're showing annotations, attempt to look up the annotation for this address.
                if (_showAnnotatedDisassembly && Annotations != null)
                {
                    var annotation = Annotations.ContainsKey(addressIndex) ? Annotations[addressIndex] : null;
                    output.Append("\t\t; ");
                    output.Append(annotation == null ? "???" : annotation);
                }

                output.AppendLine();

                // If the opcode is larger than a single byte, we don't want to print subsequent
                // bytes as opcodes, so here we print the next address locations as the byte value
                // in parentheses, and then increment so we can skip disassembly of the data.
                if (instructionLength == 3)
                {
                    var upper = _cpu.Memory[addressIndex + 2] << 8;
                    var lower = _cpu.Memory[addressIndex + 1];
                    var combined = (UInt16)(upper | lower);
                    var dataFormatted = String.Format("0x{0:X4}", combined);
                    var address1Formatted = String.Format("0x{0:X4}", addressIndex+1);
                    var address2Formatted = String.Format("0x{0:X4}", addressIndex+2);
                    output.AppendLine($"\t{address1Formatted}\t(D16: {dataFormatted})");
                    output.AppendLine($"\t{address2Formatted}\t");
                    i += 2;
                }
                else if (instructionLength == 2)
                {
                    var dataFormatted = String.Format("0x{0:X2}", _cpu.Memory[addressIndex+1]);
                    var addressFormatted = String.Format("0x{0:X4}", addressIndex+1);
                    output.AppendLine($"\t{addressFormatted}\t(D8: {dataFormatted})");
                    i++;
                }
            }

            Console.WriteLine(output.ToString());
        }

        /**
         * Used to print the disassembly of the memory locations around where the program counter is pointing.
         * Useful when a debugger is attached.
         */
        private void PrintCurrentExecution(bool annotate = false, int beforeCount = 10, int afterCount = 10)
        {
            PrintMemory(_cpu.ProgramCounter, annotate, beforeCount, afterCount);
        }

        #endregion
    }
}
