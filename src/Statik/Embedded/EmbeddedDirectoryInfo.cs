using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Statik.Embedded
{
    internal class EmbeddedDirectoryInfo : IFileInfo
    {
        private readonly string _path;

        public EmbeddedDirectoryInfo(string path)
        {
            _path = path;
        }

        public bool Exists => true;

        public long Length => -1;

        public string PhysicalPath => null;

        public string Name => Path.GetFileName(_path);

        public DateTimeOffset LastModified => DateTimeOffset.Now;

        public bool IsDirectory => true;

        public Stream CreateReadStream()
        {
            throw new InvalidOperationException("Cannot create a stream for a directory.");
        }
    }
}