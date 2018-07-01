using System;
using System.Diagnostics;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Xunit;

namespace Statik.Pages.Tests
{
    public class PageDirectoryLoaderTests : IDisposable
    {
        readonly IPageDirectoryLoader _pageDirectoryLoader;
        readonly string _directory;
        
        public PageDirectoryLoaderTests()
        {
            var services = new ServiceCollection();
            Statik.RegisterServices(services);
            StatikPages.RegisterServices(services);
            _pageDirectoryLoader = services.BuildServiceProvider().GetRequiredService<IPageDirectoryLoader>();
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
            treeItem.Children.Count.Should().Be(1);
            treeItem.Children[0].Data.Name.Should().Be("file.md");
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

            treeItem.Path.Should().Be("/");
            treeItem.Data.Name.Should().Be("index.md");
            treeItem.Children.Count.Should().Be(2);
            treeItem.Children[0].Path.Should().Be("/");
            treeItem.Children[0].Data.Name.Should().Be("file.md");
            treeItem.Children[0].Data.IsDirectory.Should().BeFalse();
            treeItem.Children[1].Path.Should().Be("/test");
            treeItem.Children[1].Data.Name.Should().Be("index.md");
            treeItem.Children[1].Children.Count.Should().Be(1);
            treeItem.Children[1].Children[0].Path.Should().Be("/test");
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