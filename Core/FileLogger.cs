using System;
using System.IO;
using WeaponGameRestApi.Core;

namespace WeaponGameRestApi.Core
{
    public interface IGameLogger
    {
        void Log(string message);
    }
    public class FileLogger : IGameLogger, IDisposable
    {
        private StreamWriter _writer;
        private bool _disposed = false;
        public FileLogger(string filePath)
        {
            _writer = new StreamWriter(filePath, true);
        }

        public void Log(string message)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("FileLogger", "Cannot write to a disposed FileLogger.");
            }
            _writer.WriteLine($"{DateTime.Now}: {message}");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _writer?.Dispose();
                _disposed = true;
            }
        }
        ~FileLogger()
        {
            Dispose(false);
        }
    }
}
