// SET PIXEL

using System;
using System.Diagnostics;
using System.Threading;
using Meadow;
using Meadow.Foundation.Displays;

namespace JustinCredible.SIEmulator.MeadowMCU
{
    public class Renderer : IDisposable
    {
        // The display device used for rendering.
        private readonly St7789 _display;

        // Synchronization primitives for the render thread.
        private readonly object _renderLock = new object();
        private readonly AutoResetEvent _renderSignal = new AutoResetEvent(false);
        private Thread _renderThread;
        private bool _renderThreadRunning;
        private bool _renderFramePending;

        // Frame buffers for rendering.
        private const int FRAME_BUFFER_SIZE = (SpaceInvaders.RESOLUTION_WIDTH * SpaceInvaders.RESOLUTION_HEIGHT) / 8;
        // The frame buffer most recently queued by the main thread.
        private byte[] _queuedFrameBuffer = new byte[FRAME_BUFFER_SIZE];
        // The frame buffer currently being rendered by the render thread.
        private byte[] _renderFrameBuffer = new byte[FRAME_BUFFER_SIZE];
        // The last frame buffer that was rendered, used for optimizing rendering by only updating changed pixels.
        private readonly byte[] _lastRenderedFrameBuffer = new byte[FRAME_BUFFER_SIZE];
        private bool _hasLastRenderedFrame;

        // Metrics and statistics for render performance and dropped frames.
        private readonly bool _enableRenderMetrics;
        private readonly bool _enableDroppedFrameWarnings;
        private const int RENDER_METRICS_WINDOW = 60;
        private int _droppedRenderFrameCount;
        private long _renderMetricsFillTicks;
        private long _renderMetricsShowTicks;
        private int _renderMetricsWindowCount;
        private int _droppedRenderFrameCountLastMetricsLog;

        // Indicates whether the renderer has been disposed.
        private bool _isDisposed;

        /**
         * Handles rendering the Space Invaders frame buffer to the physical display in a separate thread.
         * Frames can be queued for rendering via the QueueFrame method, which will signal the render thread to process them.
         */
        public Renderer(St7789 display, bool enableRenderMetrics, bool enableDroppedFrameWarnings)
        {
            _display = display ?? throw new ArgumentNullException(nameof(display));
            _enableRenderMetrics = enableRenderMetrics;
            _enableDroppedFrameWarnings = enableDroppedFrameWarnings;
        }

        /**
         * Starts the render thread, which will wait for frames to be queued and then render them to the display.
         */
        public void Start()
        {
            ThrowIfDisposed();

            lock (_renderLock)
            {
                if (_renderThreadRunning)
                    return;

                _renderThreadRunning = true;
            }

            _renderThread = new Thread(new ThreadStart(RenderLoop));
            _renderThread.Name = "Emulator: Render Loop";
            _renderThread.Start();
        }

        /**
         * Stops the render thread and waits for it to finish.
         */
        public void Stop()
        {
            ThrowIfDisposed();
            StopInternal();
        }

        /**
         * Queues a frame for rendering. If a frame is already pending, this frame will be dropped.
         * @param frameBuffer The frame buffer to be rendered (from the SpaceInvaders.OnRender event).
         */
        public void QueueFrame(byte[] frameBuffer)
        {
            ThrowIfDisposed();

            lock (_renderLock)
            {
                // If a frame is already pending, we'll drop this one.
                // We may also need to do some bookkeeping for metrics and warnings.
                if (_renderFramePending)
                {
                    if (_enableRenderMetrics || _enableDroppedFrameWarnings)
                        _droppedRenderFrameCount++;

                    if (_enableDroppedFrameWarnings && _droppedRenderFrameCount % 120 == 0)
                        Resolver.Log.Info($"[WARN] Dropped {_droppedRenderFrameCount} render frames so far");

                    return;
                }

                // Copy the new frame into the queued frame buffer and signal the render thread to process it.
                Array.Copy(frameBuffer, _queuedFrameBuffer, FRAME_BUFFER_SIZE);
                _renderFramePending = true;
            }

            _renderSignal.Set();
        }

        /**
         * Disposes the renderer by stopping the render thread and cleaning up resources.
         * Once disposed, the renderer cannot be used again.
         */
        public void Dispose()
        {
            if (_isDisposed)
                return;

            // Stop the render worker before disposing synchronization primitives.
            StopInternal();
            _renderSignal.Dispose();
            _isDisposed = true;
        }

        private void StopInternal()
        {
            Thread threadToJoin = null;

            lock (_renderLock)
            {
                if (!_renderThreadRunning)
                    return;

                _renderThreadRunning = false;
                threadToJoin = _renderThread;
            }

            _renderSignal.Set();
            threadToJoin?.Join();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(Renderer));
        }

        #region Rendering Loop and Logic

        /**
         * The main loop for the render thread, which waits for frames to be queued and then renders them to the display.
         */
        private void RenderLoop()
        {
            while (_renderThreadRunning)
            {
                // Wait for a signal that a new frame is ready to be rendered, or that we should stop.
                _renderSignal.WaitOne(100);

                if (!_renderThreadRunning)
                    break;

                lock (_renderLock)
                {
                    // If no frame is pending, just continue waiting.
                    if (!_renderFramePending)
                        continue;

                    // Swap the queued frame buffer with the render frame buffer so that the render thread can work on it
                    // without blocking the main thread from queuing another frame.
                    var temp = _renderFrameBuffer;
                    _renderFrameBuffer = _queuedFrameBuffer;
                    _queuedFrameBuffer = temp;
                    _renderFramePending = false;
                }

                // Do any manipulation and tell the display to render the new frame.
                RenderFrame(_renderFrameBuffer);
            }
        }

        /**
         * Renders a frame by comparing the new frame buffer to the last rendered frame buffer and only updating pixels that have changed.
         * This minimizes the amount of data sent to the display and can improve performance, especially on slower interfaces.
         */
        private void RenderFrame(byte[] frameBuffer)
        {
            // Optional timing start for the buffer-diff + pixel-write phase.

            var fillStartTimestamp = 0L;

            if (_enableRenderMetrics)
                fillStartTimestamp = Stopwatch.GetTimestamp();

            var pixelNumber = 0;

            // Walk the 1bpp frame buffer, update only changed pixels, and track a dirty rectangle.
            for (var byteIndex = 0; byteIndex < frameBuffer.Length; byteIndex++)
            {
                var previousValue = _hasLastRenderedFrame ? _lastRenderedFrameBuffer[byteIndex] : (byte)0;
                var currentValue = frameBuffer[byteIndex];
                // XOR gives us a 1-bit wherever the pixel state changed since the last rendered frame.
                var changedBits = previousValue ^ currentValue;

                if (changedBits == 0)
                {
                    // No changes in this byte, so skip all 8 pixels at once.
                    pixelNumber += 8;
                    continue;
                }

                // Compare at bit granularity so we only touch pixels that actually changed.
                for (var bit = 0; bit < 8; bit++)
                {
                    var mask = (byte)(1 << bit);

                    if ((changedBits & mask) != 0)
                    {
                        // Map linear framebuffer position to rotated display coordinates.
                        var x = pixelNumber / SpaceInvaders.RESOLUTION_WIDTH;
                        var y = (SpaceInvaders.RESOLUTION_WIDTH - 1) - (pixelNumber % SpaceInvaders.RESOLUTION_WIDTH);

                        // Crop the Space Invaders frame to whatever visible region the panel supports.
                        if (x < _display.Width && y < _display.Height)
                        {
                            var color = (currentValue & mask) != 0
                                ? GetOverlayColorForY(y)
                                : Color.Black;

                            _display.PixelBuffer.SetPixel(x, y, color);
                        }
                    }

                    pixelNumber++;
                }
            }

            // Persist current frame as the baseline for the next diff pass.
            Array.Copy(frameBuffer, _lastRenderedFrameBuffer, FRAME_BUFFER_SIZE);
            _hasLastRenderedFrame = true;

            // Statistics for the buffer-diff + pixel-write phase.

            var showStartTimestamp = 0L;

            if (_enableRenderMetrics)
                showStartTimestamp = Stopwatch.GetTimestamp();

            _display.Show();

            // Optional rolling render metrics (fill, show, total, dropped frames).
            if (_enableRenderMetrics)
            {
                var endTimestamp = Stopwatch.GetTimestamp();
                _renderMetricsFillTicks += (showStartTimestamp - fillStartTimestamp);
                _renderMetricsShowTicks += (endTimestamp - showStartTimestamp);
                _renderMetricsWindowCount++;

                if (_renderMetricsWindowCount >= RENDER_METRICS_WINDOW)
                {
                    var fillAverageMs = ((_renderMetricsFillTicks * 1000.0) / Stopwatch.Frequency) / _renderMetricsWindowCount;
                    var showAverageMs = ((_renderMetricsShowTicks * 1000.0) / Stopwatch.Frequency) / _renderMetricsWindowCount;
                    var droppedSinceLastLog = _droppedRenderFrameCount - _droppedRenderFrameCountLastMetricsLog;
                    var totalAverageMs = fillAverageMs + showAverageMs;
                    var totalWindowMs = ((_renderMetricsFillTicks + _renderMetricsShowTicks) * 1000.0) / Stopwatch.Frequency;
                    var renderedFrameCount = _renderMetricsWindowCount;
                    var sourceFrameCount = renderedFrameCount + droppedSinceLastLog;
                    var renderedFps = totalWindowMs > 0
                        ? (renderedFrameCount * 1000.0) / totalWindowMs
                        : 0;
                    var sourceFpsEstimate = totalWindowMs > 0
                        ? (sourceFrameCount * 1000.0) / totalWindowMs
                        : 0;
                    var dropPercent = sourceFrameCount > 0
                        ? (droppedSinceLastLog * 100.0) / sourceFrameCount
                        : 0;

                    Resolver.Log.Info($"[RENDER] Avg over {_renderMetricsWindowCount} rendered frames: fill={fillAverageMs:F2} ms, show={showAverageMs:F2} ms, total={totalAverageMs:F2} ms, renderedFps={renderedFps:F2}, sourceFps~={sourceFpsEstimate:F2}, dropped={droppedSinceLastLog} ({dropPercent:F1}%)");

                    _droppedRenderFrameCountLastMetricsLog = _droppedRenderFrameCount;
                    _renderMetricsFillTicks = 0;
                    _renderMetricsShowTicks = 0;
                    _renderMetricsWindowCount = 0;
                }
            }
        }

        private static Color GetOverlayColorForY(int y)
        {
            if (y >= 182 && y <= 223)
                return Color.Green; // Player ship and shields

            if (y >= 33 && y <= 55)
                return Color.Red; // UFO and explosions at top of screen

            return Color.White; // Everything else; invaders, projectiles, and most explosions.
        }

        #endregion
    }
}
