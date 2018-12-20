using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Statik.Hosting;
using Statik.Hosting.Impl;
using Statik.Web;
using Xunit;

namespace Statik.Tests
{
    public class ExporterTests : IDisposable
    {
        readonly IWebBuilder _webBuilder;
        readonly IHostExporter _hostExporter;
        readonly string _directory;

        public ExporterTests()
        {
            _webBuilder = new Web.Impl.WebBuilder(new HostBuilder());
            _hostExporter = new HostExporter();
            _directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString());
            Directory.CreateDirectory(_directory);
        }
        
        [Fact]
        public async Task Can_export_path_without_extension()
        {
            _webBuilder.Register("/test",
                context => context.Response.WriteAsync("test content"));
            using (var host = _webBuilder.BuildVirtualHost())
            {
                await _hostExporter.Export(host, _directory);
            }

            File.Exists(Path.Combine(_directory, "test", "index.html")).Should().BeTrue();
        }
        
        [Fact]
        public async Task Can_export_path_with_extension()
        {
            _webBuilder.Register("/test.css",
                context => context.Response.WriteAsync("test content"));
            using (var host = _webBuilder.BuildVirtualHost())
            {
                await _hostExporter.Export(host, _directory);
            }

            File.Exists(Path.Combine(_directory, "test.css")).Should().BeTrue();
            File.ReadAllText(Path.Combine(_directory, "test.css")).Should().Be("test content");
        }
        
        [Fact]
        public async Task Can_export_path_without_extension_as_exact_path()
        {
            _webBuilder.Register("/test",
                context => context.Response.WriteAsync("test content"),
                extractExactPath: true);
            using (var host = _webBuilder.BuildVirtualHost())
            {
                await _hostExporter.Export(host, _directory);
            }

            File.Exists(Path.Combine(_directory, "test")).Should().BeTrue();
            File.ReadAllText(Path.Combine(_directory, "test")).Should().Be("test content");
        }

        [Fact]
        public async Task Can_export_parallel()
        {
            for (var x = 0; x < 40; x++)
            {
                var _ = x;
                _webBuilder.Register($"/test{_}.txt", async context => { await context.Response.WriteAsync($"test content {_}"); });
            }
            
            using (var host = _webBuilder.BuildVirtualHost())
            {
                await _hostExporter.ExportParallel(host, _directory);
            }
            
            for (var x = 0; x < 40; x++)
            {
                File.Exists(Path.Combine(_directory, $"test{x}.txt")).Should().BeTrue();
                File.ReadAllText(Path.Combine(_directory, $"test{x}.txt")).Should().Be($"test content {x}");
            }
        }
        

        public void Dispose()
        {
            Directory.Delete(_directory, true);
        }
    }
}