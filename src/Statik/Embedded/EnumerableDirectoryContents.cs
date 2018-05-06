using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Statik.Embedded
{
    internal class EnumerableDirectoryContents : IDirectoryContents
    {
        readonly List<EmbeddedFile> _entries;
        readonly string _subPath;
        private readonly Assembly _assembly;
        List<IFileInfo> _matched = null;

        public EnumerableDirectoryContents(List<EmbeddedFile> entries, string subPath, Assembly assembly)
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
                
                foreach (var entry in _entries.Where(x => x.Path.StartsWith(_subPath)))
                {
                    var remaining = entry.Path.Substring(_subPath.Length);
                    if (remaining.TrimStart('/').Contains("/"))
                    {
                        // TODO:
                        // this is a directory
                    }
                    else
                    {
                        matched.Add(new EmbeddedFileInfo(entry, _assembly));
                    }
                }

                _matched = matched;
            }
        }
    }
}