using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Statik.Pages;
using Xunit;

namespace Statik.Tests
{
    public class PageDirectoryLoaderTests : IDisposable
    {
        readonly IPageDirectoryLoader _pageDirectoryLoader;
        readonly string _directory;
        
        public PageDirectoryLoaderTests()
        {
            _pageDirectoryLoader = Statik.GetPageDirectoryLoader();
            _directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString());
            Directory.CreateDirectory(_directory);
        }
        
        [Fact]
        public void Can_load_pages()
        {
            File.WriteAllText(Path.Combine(_directory, "index.md"), "content");
            File.WriteAllText(Path.Combine(_directory, "file.md"), "content");
            File.WriteAllText(Path.Combine(_directory, "test.txt"), "content");
            
            var treeItem = _pageDirectoryLoader.LoadFiles(new PhysicalFileProvider(_directory),
                "*.md",
                "index.md");

            treeItem.Data.Name.Should().Be("index.md");
            treeItem.IsIndex.Should().BeTrue();
            treeItem.Children.Count.Should().Be(1);
            treeItem.Children[0].Data.Name.Should().Be("file.md");
            treeItem.Children[0].IsIndex.Should().BeFalse();
        }

        [Fact]
        public void Can_load_nested_pages()
        {
            File.WriteAllText(Path.Combine(_directory, "index.md"), "content");
            File.WriteAllText(Path.Combine(_directory, "file.md"), "content");
            Directory.CreateDirectory(Path.Combine(_directory, "test"));
            File.WriteAllText(Path.Combine(_directory, "test", "index.md"), "content");
            File.WriteAllText(Path.Combine(_directory, "test", "file2.md"), "content");
            
            var treeItem = _pageDirectoryLoader.LoadFiles(new PhysicalFileProvider(_directory),
                "*.md",
                "index.md");

            treeItem.BasePath.Should().Be("");
            treeItem.Data.Name.Should().Be("index.md");
            treeItem.Children.Count.Should().Be(2);
            treeItem.Children[0].BasePath.Should().Be("");
            treeItem.Children[0].Data.Name.Should().Be("file.md");
            treeItem.Children[0].Data.IsDirectory.Should().BeFalse();
            treeItem.Children[1].BasePath.Should().Be("/test");
            treeItem.Children[1].Data.Name.Should().Be("index.md");
            treeItem.Children[1].Children.Count.Should().Be(1);
            treeItem.Children[1].Children[0].BasePath.Should().Be("/test");
            treeItem.Children[1].Children[0].Data.Name.Should().Be("file2.md");
            treeItem.Children[1].Children[0].Data.IsDirectory.Should().BeFalse();
        }

        [Fact]
        public void Can_create_empty_node_if_intermediate_child_has_no_page()
        {
            File.WriteAllText(Path.Combine(_directory, "index.md"), "content");
            Directory.CreateDirectory(Path.Combine(_directory, "child", "grandchild"));
            File.WriteAllText(Path.Combine(_directory, "child", "grandchild", "index.md"), "content");
            
            var treeItem = _pageDirectoryLoader.LoadFiles(new PhysicalFileProvider(_directory),
                "*.md",
                "index.md");

            treeItem.Data.Name.Should().Be("index.md");
            treeItem.Children.Count.Should().Be(1);
            treeItem.Children[0].Data.IsDirectory.Should().Be(true);
            treeItem.Children[0].Data.Name.Should().Be("child");
            treeItem.Children[0].Children.Count.Should().Be(1);
            treeItem.Children[0].Children[0].Data.Name.Should().Be("index.md");
        }
        
        public void Dispose()
        {
            Directory.Delete(_directory, true);
        }
    }
}