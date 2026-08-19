using System;

namespace DiplomWork_Ivan_2026.Trends
{
    public static class TrendTimeFormatter
    {
        public static string FormatAxisTick(
            double elapsedSeconds,
            double visibleSpanSeconds,
            double maximumElapsedSeconds)
        {
            TimeDisplayMode mode = SelectMode(
                visibleSpanSeconds,
                maximumElapsedSeconds);
            return Format(elapsedSeconds, mode);
        }

        public static string GetAxisTitle(
            double visibleSpanSeconds,
            double maximumElapsedSeconds)
        {
            return SelectMode(visibleSpanSeconds, maximumElapsedSeconds) switch
            {
                TimeDisplayMode.MinutesSeconds => "Elapsed time [mm:ss]",
                TimeDisplayMode.HoursMinutesSeconds => "Elapsed time [h:mm:ss]",
                TimeDisplayMode.HoursMinutes => "Elapsed time [h:mm]",
                _ => "Elapsed time [d hh:mm]"
            };
        }

        public static string FormatCursor(double elapsedSeconds)
        {
            return elapsedSeconds >= 24.0 * 3600.0
                ? Format(elapsedSeconds, TimeDisplayMode.DaysHoursMinutesSeconds)
                : Format(elapsedSeconds, TimeDisplayMode.HoursMinutesSeconds);
        }

        private static TimeDisplayMode SelectMode(
            double visibleSpanSeconds,
            double maximumElapsedSeconds)
        {
            if (visibleSpanSeconds <= 15.0 * 60.0)
            {
                return maximumElapsedSeconds < 3600.0
                    ? TimeDisplayMode.MinutesSeconds
                    : maximumElapsedSeconds < 24.0 * 3600.0
                        ? TimeDisplayMode.HoursMinutesSeconds
                        : TimeDisplayMode.DaysHoursMinutesSeconds;
            }

            if (maximumElapsedSeconds >= 24.0 * 3600.0)
                return TimeDisplayMode.DaysHoursMinutes;

            return TimeDisplayMode.HoursMinutes;
        }

        private static string Format(
            double elapsedSeconds,
            TimeDisplayMode mode)
        {
            TimeSpan value = TimeSpan.FromSeconds(
                Math.Max(0.0, elapsedSeconds));
            int totalHours = (int)Math.Floor(value.TotalHours);
            int totalMinutes = (int)Math.Floor(value.TotalMinutes);

            return mode switch
            {
                TimeDisplayMode.MinutesSeconds =>
                    $"{totalMinutes:D2}:{value.Seconds:D2}",
                TimeDisplayMode.HoursMinutesSeconds =>
                    $"{totalHours}:{value.Minutes:D2}:{value.Seconds:D2}",
                TimeDisplayMode.HoursMinutes =>
                    $"{totalHours}:{value.Minutes:D2}",
                TimeDisplayMode.DaysHoursMinutesSeconds =>
                    $"{(int)value.TotalDays}d {value.Hours:D2}:{value.Minutes:D2}:{value.Seconds:D2}",
                _ =>
                    $"{(int)value.TotalDays}d {value.Hours:D2}:{value.Minutes:D2}"
            };
        }

        private enum TimeDisplayMode
        {
            MinutesSeconds,
            HoursMinutesSeconds,
            HoursMinutes,
            DaysHoursMinutes,
            DaysHoursMinutesSeconds
        }
    }
}
