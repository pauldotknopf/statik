using System.Linq;
using Microsoft.Extensions.FileProviders;
using Statik.Embedded;
using Xunit;

namespace Statik.Tests
{
    public class EmbeddedFileProviderTests
    {
        private IFileProvider _fileProvider;
        
        public EmbeddedFileProviderTests()
        {
            _fileProvider = new EmbeddedFileProvider(typeof(EmbeddedFileProviderTests).Assembly, "Statik.Tests.Embedded");
        }

        [Fact]
        public void Can_get_valid_file()
        {
            var file = _fileProvider.GetFileInfo("/file1.txt");
            
            Assert.True(file.Exists);
            Assert.Equal(file.Name, "file1.txt");
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
    }
}