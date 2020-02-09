using System;
using System.Collections.Generic;
using System.Diagnostics;
using SDL2;
using static SDL2.SDL;

namespace JustinCredible.SIEmulator
{
    class GUI : IDisposable
    {
        private IntPtr _window = IntPtr.Zero;
        private IntPtr _renderer = IntPtr.Zero;
        private int _targetFPS = 15;

        public delegate void TickEvent(GUITickEventArgs e);
        public event TickEvent OnTick;

        public void Initialize(string title, int width = 640, int height = 480, float scaleX = 1, float scaleY = 1, int targetFPS = 60)
        {
            var initResult = SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            if (initResult < 0)
                throw new Exception(String.Format("Failure while initializing SDL. Error: {0}", SDL.SDL_GetError()));

            _window = SDL.SDL_CreateWindow(title,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                (int)(width * scaleX),
                (int)(height * scaleY),
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE
            );

            if (_window == IntPtr.Zero)
                throw new Exception(String.Format("Unable to create a window. SDL Error: {0}", SDL.SDL_GetError()));

            _renderer = SDL.SDL_CreateRenderer(_window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED /*| SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC*/);

            if (_renderer == IntPtr.Zero)
                throw new Exception(String.Format("Unable to create a renderer. SDL Error: {0}", SDL.SDL_GetError()));

            SDL.SDL_RenderSetScale(_renderer, scaleX, scaleY);

            _targetFPS = targetFPS;
        }

        public void StartLoop()
        {
            // Used to keep track of the time elapsed in each loop iteration. This is used to
            // notify the OnTick handlers so they can update their simulation, as well as throttle
            // the update loop to 60hz if needed.
            var stopwatch = new Stopwatch();

            // Structure used to pass data to and from the OnTick handlers. We initialize it once
            // outside of the loop to avoid eating a ton of memory putting GC into a tailspin.
            var tickEventArgs = new GUITickEventArgs();
            tickEventArgs.Keys = GetEmptyKeyDictionary();

            // The SDL event polled for in each iteration of the loop.
            SDL.SDL_Event sdlEvent;

            while (true)
            {
                stopwatch.Restart();

                tickEventArgs.KeyDown = null;

                while (SDL.SDL_PollEvent(out sdlEvent) != 0)
                {
                    switch (sdlEvent.type)
                    {
                        // e.g. Command + Q
                        case SDL.SDL_EventType.SDL_QUIT:
                            // Break out of the SDL event loop, which will close the program.
                            return;

                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            tickEventArgs.KeyDown = sdlEvent.key.keysym.sym;
                            UpdateKeys(tickEventArgs.Keys, sdlEvent.key.keysym.sym, true);
                            break;

                        case SDL.SDL_EventType.SDL_KEYUP:
                            UpdateKeys(tickEventArgs.Keys, sdlEvent.key.keysym.sym, false);
                            break;
                    }
                }

                // Update the event arguments that will be sent with the event handler.

                tickEventArgs.ShouldRender = false;

                // Delegate out to the event handler so work can be done.
                if (OnTick != null)
                    OnTick(tickEventArgs);

                // We only want to re-render if the frame buffer has changed since last time because
                // the SDL_RenderPresent method is relatively expensive and massively slows down the
                // amount of opcodes (ticks) we can execute.
                if (tickEventArgs.ShouldRender)
                {
                    // Render screen from the updated the frame buffer.
                    // NOTE: The electron beam scans from left to right, starting in the upper left corner
                    // of the CRT when it is in 4:3 (landscape), which is how the framebuffer is stored.
                    // However, since the CRT in the cabinet is rotated left (-90 degrees) to show the game
                    // in 3:4 (portrait) we need to perform the rotation of points below by starting in the
                    // bottom left corner of the window and drawing upwards, ending on the top right.

                    var frameBuffer = tickEventArgs.FrameBuffer;

                    if (frameBuffer != null)
                    {
                        // Clear the screen.
                        SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 0);
                        SDL.SDL_RenderClear(_renderer);

                        var bits = new System.Collections.BitArray(frameBuffer);

                        var x = 0;
                        var y = SpaceInvaders.RESOLUTION_WIDTH - 1;

                        for (var i = 0; i < bits.Length; i++)
                        {
                            if (bits[i])
                            {
                                // The CRT is black/white and the framebuffer is 1-bit per pixel.
                                // A transparent overlay added "colors" to areas of the CRT. These
                                // are the approximate y locations of each area/color of the overlay:
                                // • 0-18: White
                                // • 18-72: Green
                                // • 73-224: White
                                // • 225-254: Red

                                if (y >= 182 && y <= 222)
                                    SDL.SDL_SetRenderDrawColor(_renderer, 0, 255, 0, 255); // Green
                                else if (y >= 0 && y <= 31)
                                    SDL.SDL_SetRenderDrawColor(_renderer, 255, 0, 0, 255); // Red
                                else
                                    SDL.SDL_SetRenderDrawColor(_renderer, 255, 255, 255, 255); // White

                                SDL.SDL_RenderDrawPoint(_renderer, x, y);
                            }

                            y--;

                            if (y == -1)
                            {
                                y = SpaceInvaders.RESOLUTION_WIDTH - 1;
                                x++;
                            }

                            if (x == SpaceInvaders.RESOLUTION_HEIGHT)
                                break;

                        }
                    }

                    SDL.SDL_RenderPresent(_renderer);
                }

                // See if we need to delay to keep locked to ~ TARGET_FPS FPS.

                if (stopwatch.Elapsed.TotalMilliseconds < (1000 / _targetFPS))
                {
                    var delay = (1000 / _targetFPS) - stopwatch.Elapsed.TotalMilliseconds;
                    SDL.SDL_Delay((uint)delay);
                }

                // If the event handler indicated we should quit, then stop.
                if (tickEventArgs.ShouldQuit)
                    return;
            }
        }

        private Dictionary<byte, bool> GetEmptyKeyDictionary()
        {
            var dictionary = new Dictionary<byte, bool>();

            for (var i = 0; i < 16; i++)
                dictionary.Add((byte)i, false);

            return dictionary;
        }

        private void UpdateKeys(Dictionary<byte, bool> keys, SDL.SDL_Keycode keycode, bool isDown)
        {
            // Used mapping from here: http://www.multigesture.net/articles/how-to-write-an-emulator-chip-8-interpreter/
            // TODO: Make mapping configurable.

            // Keypad                   Keyboard
            // +-+-+-+-+                +-+-+-+-+
            // |1|2|3|C|                |1|2|3|4|
            // +-+-+-+-+                +-+-+-+-+
            // |4|5|6|D|                |Q|W|E|R|
            // +-+-+-+-+       =>       +-+-+-+-+
            // |7|8|9|E|                |A|S|D|F|
            // +-+-+-+-+                +-+-+-+-+
            // |A|0|B|F|                |Z|X|C|V|
            // +-+-+-+-+                +-+-+-+-+

            switch (keycode)
            {
                case SDL.SDL_Keycode.SDLK_1:
                    keys[0x01] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_2:
                    keys[0x02] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_3:
                    keys[0x03] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_4:
                    keys[0x0C] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_q:
                    keys[0x04] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_w:
                    keys[0x05] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_e:
                    keys[0x06] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_r:
                    keys[0x0D] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_a:
                    keys[0x07] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_s:
                    keys[0x08] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_d:
                    keys[0x09] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_f:
                    keys[0x0E] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_z:
                    keys[0x0A] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_x:
                    keys[0x00] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_c:
                    keys[0x0B] = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_v:
                    keys[0x0F] = isDown;
                    break;
            }
        }

        public void Dispose()
        {
            if (_renderer != IntPtr.Zero)
                SDL.SDL_DestroyRenderer(_renderer);

            if (_window != IntPtr.Zero)
                SDL.SDL_DestroyWindow(_window);
        }
    }
}
