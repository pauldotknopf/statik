using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Statik.Embedded.Impl;

namespace Statik.Embedded
{
    public class EmbeddedFileProvider : IFileProvider
    {
        static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars()
            .Where(c => c != '/' && c != '\\').ToArray();
        readonly IAssemblyResourceResolver _assembly;
        readonly List<EmbeddedFile> _files;

        public EmbeddedFileProvider(IAssemblyResourceResolver assembly, string prefix)
        {
            _assembly = assembly;
            _files = _assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith(prefix))
                .Select(x =>
                {
                    // This bit will will create convert the resource name
                    // "this.is.a.resource.jpg" to a path of "/this/is/a/resource.jpg".
                    var parts = x.Substring(prefix.Length).Split('.');
                    var directory = parts.Take(parts.Length - 2);
                    var fileName = parts.Skip(parts.Length - 2);
                    var path = Path.DirectorySeparatorChar + Path.Combine(Path.Combine(directory.ToArray()), string.Join(".", fileName));
                    
                    return new EmbeddedFile(path, x);
                })
                .ToList();
        }
        
        public EmbeddedFileProvider(Assembly assembly, string prefix)
            :this(new AssemblyResourceResolver(assembly), prefix)
        {
            
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (string.IsNullOrEmpty(subpath)) return new NotFoundFileInfo(subpath);
            
            var file = _files.SingleOrDefault(x => x.Path.Equals(subpath));

            if (file == null) return new NotFoundFileInfo(subpath);
            
            return new EmbeddedFileInfo(file, _assembly);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (string.IsNullOrEmpty(subpath)) return NotFoundDirectoryContents.Singleton;

            return new EnumerableDirectoryContents(_files, subpath, _assembly);
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
        
        private static bool HasInvalidPathChars(string path)
        {
            return path.IndexOfAny(InvalidFileNameChars) != -1;
        }
    }
}