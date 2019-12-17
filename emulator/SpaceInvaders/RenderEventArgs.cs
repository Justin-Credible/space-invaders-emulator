using System;
using System.Collections.Generic;
using SDL2;

namespace JustinCredible.SIEmulator
{
    public class RenderEventArgs : EventArgs
    {
        public byte[] FrameBuffer { get; set; }
        public bool ShouldRender { get; set; }
    }
}
