using System;
using System.Collections.Generic;

namespace JustinCredible.SIEmulator.MeadowMCU
{
    public class Configuration
    {
        private readonly bool _enableEmulationStats;
        private readonly bool _enableRenderMetrics;
        private readonly bool _enableDroppedFrameWarnings;
        private readonly int _haltAfterRecordedStatCount;
        private readonly StartingShipsSetting _startingShips;
        private readonly ExtraShipAtSetting _extraShipAt;

        public bool EnableEmulationStats => _enableEmulationStats;
        public bool EnableRenderMetrics => _enableRenderMetrics;
        public bool EnableDroppedFrameWarnings => _enableDroppedFrameWarnings;
        public int HaltAfterRecordedStatCount => _haltAfterRecordedStatCount;
        public StartingShipsSetting StartingShips => _startingShips;
        public ExtraShipAtSetting ExtraShipAt => _extraShipAt;

        public Configuration(Dictionary<string, string> settings, Action<string> onWarning = null)
        {
            _enableEmulationStats = GetBooleanSetting(settings, "Diagnostics.EnableEmulationStats", false, onWarning);
            _enableRenderMetrics = GetBooleanSetting(settings, "Diagnostics.EnableRenderMetrics", false, onWarning);
            _enableDroppedFrameWarnings = GetBooleanSetting(settings, "Diagnostics.EnableDroppedFrameWarnings", false, onWarning);
            _haltAfterRecordedStatCount = GetIntegerSetting(settings, "Diagnostics.HaltAfterRecordedStatCount", -1, onWarning);
            _startingShips = GetStartingShipsSetting(settings, onWarning);
            _extraShipAt = GetExtraShipAtSetting(settings, onWarning);
        }

        private static bool GetBooleanSetting(Dictionary<string, string> settings, string key, bool defaultValue, Action<string> onWarning)
        {
            if (!settings.TryGetValue(key, out var value))
                return defaultValue;

            if (!bool.TryParse(value, out var parsedValue))
            {
                onWarning?.Invoke($"Invalid boolean value for '{key}': '{value}'. Using default value '{defaultValue}'.");
                return defaultValue;
            }

            return parsedValue;
        }

        private static int GetIntegerSetting(Dictionary<string, string> settings, string key, int defaultValue, Action<string> onWarning)
        {
            if (!settings.TryGetValue(key, out var value))
                return defaultValue;

            if (!int.TryParse(value, out var parsedValue))
            {
                onWarning?.Invoke($"Invalid integer value for '{key}': '{value}'. Using default value '{defaultValue}'.");
                return defaultValue;
            }

            return parsedValue;
        }

        private static StartingShipsSetting GetStartingShipsSetting(Dictionary<string, string> settings, Action<string> onWarning)
        {
            if (!settings.TryGetValue("Game.StartingShips", out var value))
                return StartingShipsSetting.Three;

            switch (value)
            {
                case "3":
                    return StartingShipsSetting.Three;
                case "4":
                    return StartingShipsSetting.Four;
                case "5":
                    return StartingShipsSetting.Five;
                case "6":
                    return StartingShipsSetting.Six;
                default:
                    onWarning?.Invoke($"Invalid value for 'Game.StartingShips': '{value}'. Allowed values are 3, 4, 5, or 6. Using default value '3'.");
                    return StartingShipsSetting.Three;
            }
        }

        private static ExtraShipAtSetting GetExtraShipAtSetting(Dictionary<string, string> settings, Action<string> onWarning)
        {
            if (!settings.TryGetValue("Game.ExtraShip", out var value))
                return ExtraShipAtSetting.Points1000;

            switch (value)
            {
                case "1000":
                    return ExtraShipAtSetting.Points1000;
                case "1500":
                    return ExtraShipAtSetting.Points1500;
                default:
                    onWarning?.Invoke($"Invalid value for 'Game.ExtraShip': '{value}'. Allowed values are 1000 or 1500. Using default value '1000'.");
                    return ExtraShipAtSetting.Points1000;
            }
        }
    }
}
