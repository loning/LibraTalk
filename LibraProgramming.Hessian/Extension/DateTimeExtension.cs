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
        
        /// <summary>
        /// Gets the Maximum value for a DateTime specifying kind.
        /// 
        /// </summary>
        /// <param name="kind">DateTimeKind to use.</param>
        /// <returns>
        /// DateTime of specified kind.
        /// </returns>
        public static DateTime GetMaxValue(DateTimeKind kind)
        {
            if (DateTimeKind.Unspecified == kind)
            {
                return new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc);
            }

            return new DateTime(DateTime.MaxValue.Ticks, kind);
        }
        
        /// <summary>
        /// Gets the Minimum value for a DateTime specifying kind.
        /// 
        /// </summary>
        /// <param name="kind">DateTimeKind to use.</param>
        /// <returns>
        /// DateTime of specified kind.
        /// </returns>
        public static DateTime GetMinValue(DateTimeKind kind)
        {
            if (DateTimeKind.Unspecified == kind)
            {
                return new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc);
            }

            return new DateTime(DateTime.MinValue.Ticks, kind);
        }

        public static DateTime Add(DateTime time, TimeSpan timespan)
        {
            if (timespan == TimeSpan.Zero)
            {
                return time;
            }

            if (timespan > TimeSpan.Zero && DateTime.MaxValue - time <= timespan)
            {
                return GetMaxValue(time.Kind);
            }

            if (timespan < TimeSpan.Zero && DateTime.MinValue - time >= timespan)
            {
                return GetMinValue(time.Kind);
            }

            return time + timespan;
        }
    }
}
