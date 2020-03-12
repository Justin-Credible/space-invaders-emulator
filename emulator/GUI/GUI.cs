using System;
using System.Collections.Generic;
using System.Diagnostics;
using SDL2;

namespace JustinCredible.SIEmulator
{
    class GUI : IDisposable
    {
        private IntPtr _window = IntPtr.Zero;
        private IntPtr _renderer = IntPtr.Zero;
        private int _targetFPS = 15;
        private Dictionary<SoundEffect, IntPtr> soundEffects = null;

        public delegate void TickEvent(GUITickEventArgs e);
        public event TickEvent OnTick;

        const int AUDIO_CHANNEL_COMMON = 0;
        const int AUDIO_CHANNEL_INVADER_MOVEMENT = 1;
        const int AUDIO_CHANNEL_UFO = 2;
        const int AUDIO_INFINITE_LOOP = -1;
        const int AUDIO_NO_LOOP = 0;

        public void Initialize(string title, int width = 640, int height = 480, float scaleX = 1, float scaleY = 1, int targetFPS = 60)
        {
            var initResult = SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO);

            if (initResult != 0)
                throw new Exception(String.Format("Failure while initializing SDL. SDL Error: {0}", SDL.SDL_GetError()));

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

        public void InitializeAudio(Dictionary<SoundEffect, String> soundEffectsFiles = null)
        {
            var audioRate = 22050; // 22.05KHz
            var audioFormat = SDL.AUDIO_S16SYS; // Unsigned 16-bit samples in the system's byte order
            var audioChannels = 1; // Mono
            var audioBuffers = 4096;

            var openAudioResult = SDL_mixer.Mix_OpenAudio(audioRate, audioFormat, audioChannels, audioBuffers);

            if (openAudioResult != 0)
                throw new Exception(String.Format("Failure while opening SDL audio mixer. SDL Error: {0}", SDL.SDL_GetError()));

            // We'll use 3 channels; see AUDIO_CHANNEL constants.
            var allocateChannelsResult = SDL_mixer.Mix_AllocateChannels(3);

            if (allocateChannelsResult == 0)
                throw new Exception(String.Format("Failure while allocating SDL audio mixer channels. SDL Error: {0}", SDL.SDL_GetError()));

            soundEffects = new Dictionary<SoundEffect, IntPtr>();

            foreach (var entry in soundEffectsFiles)
            {
                var sfx = entry.Key;
                var filePath = entry.Value;

                var pointer = SDL_mixer.Mix_LoadWAV(filePath);

                if (pointer == null)
                    throw new Exception(String.Format("Error loading sound {0}. SDL Error: {1}", sfx, SDL.SDL_GetError()));

                soundEffects.Add(sfx, pointer);
            }
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

            // The SDL event polled for in each iteration of the loop.
            SDL.SDL_Event sdlEvent;

            while (true)
            {
                stopwatch.Restart();

                tickEventArgs.KeyDown = null;
                tickEventArgs.ShouldBreak = false;

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
                            UpdateKeys(tickEventArgs, sdlEvent.key.keysym.sym, true);

                            if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_PAUSE)
                                tickEventArgs.ShouldBreak = true;

                            break;

                        case SDL.SDL_EventType.SDL_KEYUP:
                            UpdateKeys(tickEventArgs, sdlEvent.key.keysym.sym, false);
                            break;
                    }
                }

                // Update the event arguments that will be sent with the event handler.

                tickEventArgs.ShouldRender = false;
                tickEventArgs.ShouldPlaySounds = false;
                tickEventArgs.SoundEffects.Clear();

                // Delegate out to the event handler so work can be done.
                if (OnTick != null)
                    OnTick(tickEventArgs);

                // We only want to re-render if the frame buffer has changed since last time because
                // the SDL_RenderPresent method is relatively expensive and massively slows down the
                // amount of opcodes (ticks) we can execute.
                if (tickEventArgs.ShouldRender && tickEventArgs.FrameBuffer != null)
                {
                    // Render screen from the updated the frame buffer.
                    // NOTE: The electron beam scans from left to right, starting in the upper left corner
                    // of the CRT when it is in 4:3 (landscape), which is how the framebuffer is stored.
                    // However, since the CRT in the cabinet is rotated left (-90 degrees) to show the game
                    // in 3:4 (portrait) we need to perform the rotation of points below by starting in the
                    // bottom left corner of the window and drawing upwards, ending on the top right.

                    // Clear the screen.
                    SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 0);
                    SDL.SDL_RenderClear(_renderer);

                    var bits = new System.Collections.BitArray(tickEventArgs.FrameBuffer);

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

                            if (y >= 182 && y <= 223)
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

                    SDL.SDL_RenderPresent(_renderer);
                }

                // Handle playing sound effects.
                if (soundEffects != null
                    && tickEventArgs.ShouldPlaySounds
                    && tickEventArgs.SoundEffects != null)
                {
                    foreach (var sfx in tickEventArgs.SoundEffects)
                    {
                        var pointer = soundEffects[sfx];

                        // Result of Mix_PlayChannel, which indicates the channel the sound is playing on.
                        // -1 indicates an error.
                        var playChannelResult = -1;

                        switch (sfx)
                        {
                            // The UFO sound effect loops until the UFO disappears or is destroyed.
                            case SoundEffect.UFO_Start:
                                playChannelResult = SDL_mixer.Mix_PlayChannel(AUDIO_CHANNEL_UFO, pointer, AUDIO_INFINITE_LOOP);
                                break;

                            case SoundEffect.UFO_Stop:
                            {
                                SDL_mixer.Mix_Pause(AUDIO_CHANNEL_UFO);

                                // Mix_Pause doesn't return a channel or error code. Ensure we don't leave as -1.
                                playChannelResult = AUDIO_CHANNEL_UFO;

                                break;
                            }

                            case SoundEffect.InvaderMove1:
                            case SoundEffect.InvaderMove2:
                            case SoundEffect.InvaderMove3:
                            case SoundEffect.InvaderMove4:
                                playChannelResult = SDL_mixer.Mix_PlayChannel(AUDIO_CHANNEL_INVADER_MOVEMENT, pointer, AUDIO_NO_LOOP);
                                break;

                            default:
                                playChannelResult = SDL_mixer.Mix_PlayChannel(AUDIO_CHANNEL_COMMON, pointer, AUDIO_NO_LOOP);
                                break;
                        }

                        if (playChannelResult == -1)
                            Console.WriteLine("Error playing sound effect {0}. SDL Error: {1}", sfx, SDL.SDL_GetError());
                    }
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

        private void UpdateKeys(GUITickEventArgs tickEventArgs, SDL.SDL_Keycode keycode, bool isDown)
        {
            switch (keycode)
            {
                case SDL.SDL_Keycode.SDLK_LEFT:
                    tickEventArgs.ButtonP1Left = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_RIGHT:
                    tickEventArgs.ButtonP1Right = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_SPACE:
                    tickEventArgs.ButtonP1Fire = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_a:
                    tickEventArgs.ButtonP2Left = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_d:
                    tickEventArgs.ButtonP2Right = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_p:
                    tickEventArgs.ButtonP2Fire = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_1:
                    tickEventArgs.ButtonStart1P = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_2:
                    tickEventArgs.ButtonStart2P = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_5:
                    tickEventArgs.ButtonCredit = isDown;
                    break;
                case SDL.SDL_Keycode.SDLK_t:
                    tickEventArgs.ButtonTilt = isDown;
                    break;
            }
        }

        public void Dispose()
        {
            if (_renderer != IntPtr.Zero)
                SDL.SDL_DestroyRenderer(_renderer);

            if (_window != IntPtr.Zero)
                SDL.SDL_DestroyWindow(_window);

            if (soundEffects != null)
            {
                foreach (var entry in soundEffects)
                {
                    var pointer = entry.Value;
                    SDL_mixer.Mix_FreeChunk(pointer);
                }

                SDL_mixer.Mix_CloseAudio();
            }
        }
    }
}
