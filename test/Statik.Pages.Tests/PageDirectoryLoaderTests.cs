using Dazinator.AspNet.Extensions.FileProviders;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Statik.Pages.Tests
{
    public class PageDirectoryLoaderTests
    {
        IPageDirectoryLoader _pageDirectoryLoader;
        
        public PageDirectoryLoaderTests()
        {
            var services = new ServiceCollection();
            Statik.RegisterServices(services);
            StatikPages.RegisterServices(services);
            _pageDirectoryLoader = services.BuildServiceProvider().GetRequiredService<IPageDirectoryLoader>();
        }
        
        [Fact]
        public void Can_load_pages()
        {
            var fileProvider = new InMemoryFileProvider();
            fileProvider.Directory.AddFile("/", new StringFileInfo("content", "index.md"));
            fileProvider.Directory.AddFile("/", new StringFileInfo("content", "file.md"));
            
            var menuItem = _pageDirectoryLoader.LoadFiles(fileProvider, "*.md", "index.md");

            menuItem.Data.Name.Should().Be("index.md");
            menuItem.Children.Count.Should().Be(1);
            menuItem.Children[0].Data.Name.Should().Be("file.md");
        }
    }
}