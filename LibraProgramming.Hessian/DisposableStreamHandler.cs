using System;
using System.IO;

namespace LibraProgramming.Hessian
{
    public class DisposableStreamHandler : IDisposable
    {
        private bool disposed;

        protected Stream Stream
        {
            get;
            private set;
        }

        protected DisposableStreamHandler(Stream stream)
        {
            Stream = stream;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
            }
        }

        protected virtual void Dispose(bool dispose)
        {
            if (!disposed)
            {
                try
                {
                    if (dispose)
                    {
                        Stream.Dispose();
                        Stream = null;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }
}