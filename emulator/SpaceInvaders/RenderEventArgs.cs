using System;

namespace JustinCredible.SIEmulator
{
    public class RenderEventArgs : EventArgs
    {
        public byte[] FrameBuffer { get; set; }
        public bool ShouldRender { get; set; }
    }
}
