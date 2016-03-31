using System;
using LibraProgramming.Hessian.Extension;

namespace LibraProgramming.Hessian
{
    internal static class EpochTime
    {
        /// <summary>
        /// DateTime as UTV for UnixEpoch
        /// 
        /// </summary>
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Per JWT spec:
        /// Gets the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until the desired date/time.
        /// 
        /// </summary>
        /// <param name="value">The DateTime to convert to seconds.</param>
        /// <remarks>
        /// if dateTimeUtc less than UnixEpoch, return 0
        /// </remarks>
        /// 
        /// <returns>
        /// the number of seconds since Unix Epoch.
        /// </returns>
        public static long GetIntDate(DateTime value)
        {
            var dt = value;

            if (value.Kind != DateTimeKind.Utc)
            {
                dt = value.ToUniversalTime();
            }

            if (dt.ToUniversalTime() <= UnixEpoch)
            {
                return 0;
            }

            return (long) (dt - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Creates a DateTime from epoch time.
        /// 
        /// </summary>
        /// <param name="secondsSinceUnixEpoch">Number of seconds.</param>
        /// <returns>
        /// The DateTime in UTC.
        /// </returns>
        public static DateTime DateTime(long secondsSinceUnixEpoch)
        {
            if (secondsSinceUnixEpoch <= 0L)
            {
                return UnixEpoch;
            }

            return DateTimeExtension
                .Add(UnixEpoch, TimeSpan.FromSeconds(secondsSinceUnixEpoch))
                .ToUniversalTime();
        }
    }
}