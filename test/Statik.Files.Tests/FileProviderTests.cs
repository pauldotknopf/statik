using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Statik.Hosting;
using Statik.Hosting.Impl;
using Statik.Tests;
using Statik.Web;
using Statik.Web.Impl;
using Statik.Files;
using Xunit;

namespace Statik.Files.Tests
{
    public class FileProviderTests
    {
        readonly IWebBuilder _webBuilder;

        public FileProviderTests()
        {
            _webBuilder = new WebBuilder(new HostBuilder());
        }
        
        [Fact]
        public async Task Can_register_files()
        {
            using (var testDirectory = new WorkingDirectorySession())
            {
                File.WriteAllText(Path.Combine(testDirectory.Directory, "test.txt"), "test");
                Directory.CreateDirectory(Path.Combine(testDirectory.Directory, "nested"));
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "test2.txt"), "test2");

                _webBuilder.RegisterDirectory(testDirectory.Directory);
                
                using(var host = _webBuilder.BuildVirtualHost())
                {
                    using(var client = host.CreateClient())
                    {
                        var responseMessage = await client.GetAsync("/test.txt");
                        responseMessage.EnsureSuccessStatusCode();
                        var response = await responseMessage.Content.ReadAsStringAsync();

                        Assert.Equal("test", response);
                        
                        responseMessage = await client.GetAsync("/nested/test2.txt");
                        responseMessage.EnsureSuccessStatusCode();
                        response = await responseMessage.Content.ReadAsStringAsync();

                        Assert.Equal("test2", response);
                    }
                }
                
                using(var host = _webBuilder.BuildVirtualHost("/appbase"))
                {
                    using(var client = host.CreateClient())
                    {
                        var responseMessage = await client.GetAsync("/test.txt");
                        responseMessage.EnsureSuccessStatusCode();
                        var response = await responseMessage.Content.ReadAsStringAsync();

                        Assert.Equal("test", response);
                        
                        responseMessage = await client.GetAsync("/nested/test2.txt");
                        responseMessage.EnsureSuccessStatusCode();
                        response = await responseMessage.Content.ReadAsStringAsync();

                        Assert.Equal("test2", response);
                    }
                }
            }
        }
        
        [Fact]
        public async Task Can_register_files_at_path()
        {
            using (var testDirectory = new WorkingDirectorySession())
            {
                File.WriteAllText(Path.Combine(testDirectory.Directory, "test.txt"), "test");
                Directory.CreateDirectory(Path.Combine(testDirectory.Directory, "nested"));
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "test2.txt"), "test2");
                
                _webBuilder.RegisterDirectory("/prefix", testDirectory.Directory);
                
                using(var host = _webBuilder.BuildVirtualHost())
                {
                    using(var client = host.CreateClient())
                    {
                        var responseMessage = await client.GetAsync("/prefix/test.txt");
                        responseMessage.EnsureSuccessStatusCode();
                        var response = await responseMessage.Content.ReadAsStringAsync();

                        Assert.Equal("test", response);
                        
                        responseMessage = await client.GetAsync("/prefix/nested/test2.txt");
                        responseMessage.EnsureSuccessStatusCode();
                        response = await responseMessage.Content.ReadAsStringAsync();

                        Assert.Equal("test2", response);
                    }
                }
                
                using(var host = _webBuilder.BuildVirtualHost("/appbase"))
                {
                    using(var client = host.CreateClient())
                    {
                        var responseMessage = await client.GetAsync("/prefix/test.txt");
                        responseMessage.EnsureSuccessStatusCode();
                        var response = await responseMessage.Content.ReadAsStringAsync();

                        Assert.Equal("test", response);
                        
                        responseMessage = await client.GetAsync("/prefix/nested/test2.txt");
                        responseMessage.EnsureSuccessStatusCode();
                        response = await responseMessage.Content.ReadAsStringAsync();

                        Assert.Equal("test2", response);
                    }
                }
            }
        }
    }
}