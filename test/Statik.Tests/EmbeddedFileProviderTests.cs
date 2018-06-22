using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Moq;
using Statik.Embedded;
using Xunit;

namespace Statik.Tests
{
    public class EmbeddedFileProviderTests
    {
        private IFileProvider _fileProvider;
        
        public EmbeddedFileProviderTests()
        {
            _fileProvider = BuildFileProvider("Statik.Tests.Embedded");
        }

        [Fact]
        public void Can_get_valid_file()
        {
            var file = _fileProvider.GetFileInfo("/file1.txt");
            
            Assert.True(file.Exists);
            Assert.Equal("file1.txt", file.Name);
        }

        [Fact]
        public void Can_get_invalid_file()
        {
            var file = _fileProvider.GetFileInfo("/non-existant.txt");
            
            Assert.False(file.Exists);
        }
        
        [Fact]
        public void Can_get_valid_directory()
        {
            var directory = _fileProvider.GetDirectoryContents("/nested");
            
            Assert.True(directory.Exists);
        }
        
        [Fact]
        public void Can_get_invalid_directory()
        {
            var directory = _fileProvider.GetDirectoryContents("/non-existant");
            
            Assert.False(directory.Exists);
        }

        [Fact]
        public void Can_get_valid_directory_with_both_file_and_directory()
        {
            var directory = _fileProvider.GetDirectoryContents("/");
            
            Assert.True(directory.Exists);

            var files = directory.ToList();
            
            Assert.Equal(3, files.Count);

            foreach (var file in files)
            {
                switch (file.Name)
                {
                    case "file1.txt":
                        Assert.False(file.IsDirectory);
                        break;
                    case "file2.txt":
                        Assert.False(file.IsDirectory);
                        break;
                    case "nested":
                        Assert.True(file.IsDirectory);
                        break;
                    default:
                        Assert.True(false, $"Invalid file name {file.Name}");
                        break;
                }
            }
            
            directory = _fileProvider.GetDirectoryContents("/nested");
            
            Assert.True(directory.Exists);

            files = directory.ToList();
            
            Assert.Equal(3, files.Count);

            foreach (var file in files)
            {
                switch (file.Name)
                {
                    case "file3.txt":
                        Assert.False(file.IsDirectory);
                        break;
                    case "file4.txt":
                        Assert.False(file.IsDirectory);
                        break;
                    case "nested2":
                        Assert.True(file.IsDirectory);
                        break;
                    default:
                        Assert.True(false, $"Invalid file name {file.Name}");
                        break;
                }
            }
        }

        [Fact]
        public void Can_get_files_at_more_root_prefix()
        {
            _fileProvider = BuildFileProvider("Statik.Tests", BuildTestAssemblyResourceResolver());

            var directory = _fileProvider.GetDirectoryContents("/");
            Assert.True(directory.Exists);

            var files = directory.ToList();
            Assert.Equal(2, files.Count);
            
            Assert.Equal("Embedded", files[0].Name);
            Assert.Equal("Some", files[1].Name);
        }
        
        private IAssemblyResourceResolver BuildTestAssemblyResourceResolver()
        {
            var assembly = new Mock<IAssemblyResourceResolver>();
            assembly.Setup(x => x.GetManifestResourceNames()).Returns(new[]
            {
                "Statik.Tests.Embedded.file1.txt",
                "Statik.Tests.Embedded.file2.txt",
                "Statik.Tests.Embedded.nested.file3.txt",
                "Statik.Tests.Embedded.nested.file4.txt",
                "Statik.Tests.Embedded.nested.nested2.file5.txt",
                "Statik.Tests.Some.Other.Prefix.file1.txt"
            });
            assembly.Setup(x => x.GetManifestResourceStream(It.IsAny<string>()))
                .Returns(new Func<string, Stream>(fileName =>
                {
                    var stream = new MemoryStream();
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(fileName);
                    }

                    return stream;
                }));
            return assembly.Object;
        }

        private IFileProvider BuildFileProvider(string prefix, IAssemblyResourceResolver assembly = null)
        {
            if (assembly == null)
                assembly = BuildTestAssemblyResourceResolver();
            return new EmbeddedFileProvider(assembly, prefix);
        }
    }
}