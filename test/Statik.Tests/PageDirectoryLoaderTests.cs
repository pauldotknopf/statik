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
        public void Can_load_with_path_set_correctly()
        {
            File.WriteAllText(Path.Combine(_directory, "index.md"), "content");
            File.WriteAllText(Path.Combine(_directory, "file.md"), "content");
            Directory.CreateDirectory(Path.Combine(_directory, "test", "nested"));
            File.WriteAllText(Path.Combine(_directory, "test", "nested", "index.md"), "content");
            File.WriteAllText(Path.Combine(_directory, "test", "nested", "file2.md"), "content");
            
            var treeItem = _pageDirectoryLoader.LoadFiles(new PhysicalFileProvider(_directory),
                "*.md",
                "index.md");

            treeItem.FilePath.Should().Be("/index.md");
            treeItem.Children[0].FilePath.Should().Be("/file.md");
            treeItem.Children[1].FilePath.Should().Be("/test");
            treeItem.Children[1].Children.Count.Should().Be(1);
            treeItem.Children[1].Children[0].FilePath.Should().Be("/test/nested/index.md");
            treeItem.Children[1].Children[0].Children.Count.Should().Be(1);
            treeItem.Children[1].Children[0].Children[0].FilePath.Should().Be("/test/nested/file2.md");
        }
        
        [Fact]
        public void Can_load_pages_with_base_path_set_correctly()
        {
            File.WriteAllText(Path.Combine(_directory, "index.md"), "content");
            File.WriteAllText(Path.Combine(_directory, "file.md"), "content");
            Directory.CreateDirectory(Path.Combine(_directory, "test"));
            File.WriteAllText(Path.Combine(_directory, "test", "index.md"), "content");
            File.WriteAllText(Path.Combine(_directory, "test", "file2.md"), "content");
            
            var treeItem = _pageDirectoryLoader.LoadFiles(new PhysicalFileProvider(_directory),
                "*.md",
                "index.md");

            treeItem.Path.Should().Be("/");
            treeItem.Children[0].Path.Should().Be("/file");
            treeItem.Children[1].Path.Should().Be("/test");
            treeItem.Children[1].Children[0].Path.Should().Be("/test/file2");
        }

        [Fact]
        public void Can_set_parent_node_properly()
        {
            File.WriteAllText(Path.Combine(_directory, "index.md"), "content");
            Directory.CreateDirectory(Path.Combine(_directory, "child", "grandchild"));
            File.WriteAllText(Path.Combine(_directory, "child", "grandchild", "index.md"), "content");
            
            var treeItem = _pageDirectoryLoader.LoadFiles(new PhysicalFileProvider(_directory),
                "*.md",
                "index.md");

            treeItem.Parent.Should().BeNull();
            treeItem.Children.Count.Should().Be(1);
            treeItem.Children[0].Parent.Should().Be(treeItem);
            treeItem.Children[0].Children.Count.Should().Be(1);
            treeItem.Children[0].Children[0].Parent.Should().Be(treeItem.Children[0]);
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

            treeItem.Path.Should().Be("/");
            treeItem.Children.Count.Should().Be(1);
            treeItem.Children[0].Data.IsDirectory.Should().Be(true);
            treeItem.Children[0].Data.Name.Should().Be("child");
            treeItem.Children[0].Children.Count.Should().Be(1);
            treeItem.Children[0].Children[0].Path.Should().Be("/child/grandchild");
        }
        
        [Fact]
        public void Can_ignore_directories_with_no_files()
        {
            File.WriteAllText(Path.Combine(_directory, "index.md"), "content");
            Directory.CreateDirectory(Path.Combine(_directory, "child", "grandchild"));
            
            var treeItem = _pageDirectoryLoader.LoadFiles(new PhysicalFileProvider(_directory),
                "*.md",
                "index.md");

            treeItem.Path.Should().Be("/");
            treeItem.Children.Should().HaveCount(0);
        }
        
        public void Dispose()
        {
            Directory.Delete(_directory, true);
        }
    }
}