using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Statik.Embedded
{
    internal class EmbeddedFileInfo : IFileInfo
    {
        readonly EmbeddedFile _file;
        readonly Assembly _assembly;

        public EmbeddedFileInfo(EmbeddedFile file, Assembly assembly)
        {
            _file = file;
            _assembly = assembly;
        }

        public bool Exists => true;

        public long Length
        {
            get
            {
                EnsureExists();

                using(var stream = _assembly.GetManifestResourceStream(_file.ResourceName))
                    // ReSharper disable PossibleNullReferenceException
                    return stream.Length;
                    // ReSharper restore PossibleNullReferenceException
            }
        }

        public string PhysicalPath
        {
            get
            {
                EnsureExists();

                return _file.ResourceName;
            }
        }

        public string Name
        {
            get
            {
                EnsureExists();

                return Path.GetFileName(_file.Path);
            }
        }

        public DateTimeOffset LastModified
        {
            get
            {
                EnsureExists();

                // TODO:
                return DateTime.Now;
            }
        }

        public bool IsDirectory => false;

        public Stream CreateReadStream()
        {
            EnsureExists();

            return _assembly.GetManifestResourceStream(_file.ResourceName);
        }

        private void EnsureExists()
        {
            if(!Exists) throw new InvalidOperationException("File doesn't exist.");
        }
    }
}