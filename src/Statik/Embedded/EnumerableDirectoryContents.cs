using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Statik.Embedded
{
    internal class EnumerableDirectoryContents : IDirectoryContents
    {
        readonly List<EmbeddedFile> _entries;
        readonly string _subPath;
        readonly IAssemblyResourceResolver _assembly;
        List<IFileInfo> _matched = null;

        public EnumerableDirectoryContents(List<EmbeddedFile> entries, string subPath, IAssemblyResourceResolver assembly)
        {
            _entries = entries;
            _subPath = subPath;
            _assembly = assembly;
        }

        public bool Exists
        {
            get
            {
                EnsureMatched();
                return _matched.Count > 0;
            }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            EnsureMatched();
            return _matched.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            EnsureMatched();
            return _entries.GetEnumerator();
        }

        private void EnsureMatched()
        {
            if (_matched == null)
            {
                var matched = new List<IFileInfo>();

                var directories = new List<string>();
                foreach (var entry in _entries.Where(x => x.Path.StartsWith(_subPath)))
                {
                    var remaining = entry.Path.Substring(_subPath.Length).TrimStart('/');
                    if (remaining.IndexOf("/", StringComparison.Ordinal) >= 0)
                    {
                        // This is a directory
                        var directory = $"/{remaining.Substring(0, remaining.IndexOf("/", StringComparison.OrdinalIgnoreCase))}";
                        if(!directories.Contains(directory))
                            directories.Add(directory);
                    }
                    else
                    {
                        matched.Add(new EmbeddedFileInfo(entry, _assembly));
                    }
                }

                foreach (var directory in directories)
                {
                    matched.Add(new EmbeddedDirectoryInfo(directory));
                }

                _matched = matched;
            }
        }
    }
}