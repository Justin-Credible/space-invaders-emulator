using System;
using System.Collections.Generic;
using SDL2;

namespace JustinCredible.SIEmulator
{
    class GUITickEventArgs : EventArgs
    {
        // Out

        // The state of the mapped keys; up or down; pre-mapped to emulator keys.
        public Dictionary<byte, bool> Keys { get; set; }

        // The keycode for a key that was pressed down _on this event loop tick only_.
        public SDL.SDL_Keycode? KeyDown { get; set; }

        // The Break/Pause key was pressed; if debugging, the emulator should break execution.
        public bool ShouldBreak { get; set; } = false;

        // In
        public byte[] FrameBuffer { get; set; }
        public bool ShouldRender { get; set; }
        public bool ShouldQuit { get; set; }
    }
}
