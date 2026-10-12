

namespace WeaponGameRestApi.Core
{
    internal class SaveToFile : IDisposable
    {
        private StreamWriter _writer;
        private bool _disposed = false;
        public SaveToFile(string filePath)
        {
            _writer = new StreamWriter(filePath, true);
        }


        public void Save(Weapon weapon)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("SaveToFile", "Cannot write to a disposed SaveToFile.");
            }
            _writer.WriteLine(weapon.SerializeToCsv());
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
        ~SaveToFile()
        {
            Dispose(false);
        }
    }
}
