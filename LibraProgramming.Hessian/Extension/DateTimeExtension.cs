using System;

namespace LibraProgramming.Hessian.Extension
{
    internal static class DateTimeExtension
    {
        private const long Era = 62135596800000L;
        private const long Millis = 60000;

        public static long GetTotalMilliseconds(this DateTime dt)
        {
            return dt.ToUniversalTime().Ticks / 10000 - Era; 
        }

        public static int GetTotalMinutes(this DateTime dt)
        {
            var val = GetTotalMilliseconds(dt);
            return (int)(val / Millis);
        }

        public static DateTime FromMinutes(int value)
        {
            var ticks = (value * Millis + Era) * 10000;
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        public static DateTime FromMilliseconds(long value)
        {
            var ticks = (value + Era) * 10000;
            return new DateTime(ticks, DateTimeKind.Utc);
        }
    }
}
