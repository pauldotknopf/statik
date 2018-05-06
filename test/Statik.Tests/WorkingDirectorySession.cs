using System;
using System.IO;

namespace Statik.Tests
{
    public class WorkingDirectorySession : IDisposable
    {
        public WorkingDirectorySession()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            while (System.IO.Directory.Exists(path))
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            System.IO.Directory.CreateDirectory(path);
            Directory = path;
        }

        public string Directory { get; }

        public void Dispose()
        {
            System.IO.Directory.Delete(Directory, true);
        }
    }
}