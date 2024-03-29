using System;
using System.Collections.Generic;
using SDL2;

namespace JustinCredible.SIEmulator
{
    class GUITickEventArgs : EventArgs
    {
        // Out

        // The state of the buttons/switches; true = pressed, false = not pressed.
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

        // The keycode for a key that was pressed down _on this event loop tick only_.
        public SDL.SDL_Keycode? KeyDown { get; set; }

        // The Break/Pause key was pressed; if debugging, the emulator should break execution.
        public bool ShouldBreak { get; set; } = false;

        // In
        public byte[] FrameBuffer { get; set; }
        public bool ShouldRender { get; set; }
        public bool ShouldPlaySounds { get; set; }
        public List<SoundEffect> SoundEffects { get; set; } = new List<SoundEffect>();
        public bool ShouldQuit { get; set; }
    }
}
