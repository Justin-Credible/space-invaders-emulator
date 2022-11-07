using System;
using System.Collections.Generic;

namespace JustinCredible.SIEmulator
{
    public class StatsEventArgs : EventArgs
    {
        /**
         * A list of the time in milliseconds it took to execute approximately 33k emulated CPU cycles,
         * which should be approximately 1/60th of a second (16.6ms) on real hardware. This can be used
         * to determine if the emulation is running too slow on the target platform.
         */
        public List<double> TimeMsToVsyncMeasurements { get; set; }
    }
}
