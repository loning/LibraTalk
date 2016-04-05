using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LibraProgramming.Communication.Server.Core
{
    public static class StreamExtensions
    {
        public static ushort ReadUInt16(this Stream reader)
        {
            if (null == reader)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var buffer = new byte[Marshal.SizeOf<ushort>()];

            if (buffer.Length != reader.Read(buffer, 0, buffer.Length))
            {
                throw new ArgumentException();
            }

            return BitConverter.ToUInt16(buffer, 0);
        }
    }
}