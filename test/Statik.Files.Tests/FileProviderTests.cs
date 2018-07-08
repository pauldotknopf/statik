using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;
using Moq;
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
        public void Can_register_files()
        {
            using (var testDirectory = new WorkingDirectorySession())
            {
                File.WriteAllText(Path.Combine(testDirectory.Directory, "file1.txt"), "test");
                File.WriteAllText(Path.Combine(testDirectory.Directory, "file2.txt"), "test");
                Directory.CreateDirectory(Path.Combine(testDirectory.Directory, "nested"));
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "file3.txt"), "test");
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "file4.txt"), "test");
                Directory.CreateDirectory(Path.Combine(testDirectory.Directory, "nested", "nested2"));
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "nested2", "file5.txt"), "test");
                
                var webBuilder = new Mock<IWebBuilder>();
                
                webBuilder.Setup(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                
                webBuilder.Object.RegisterDirectory(testDirectory.Directory);
                
                webBuilder.Verify(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
                webBuilder.Verify(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
                webBuilder.Verify(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
                webBuilder.Verify(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
                webBuilder.Verify(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
            }
        }

        [Fact]
        public void Can_register_files_with_include()
        {
            using (var testDirectory = new WorkingDirectorySession())
            {
                File.WriteAllText(Path.Combine(testDirectory.Directory, "file1.txt"), "test");
                File.WriteAllText(Path.Combine(testDirectory.Directory, "file2.txt"), "test");
                Directory.CreateDirectory(Path.Combine(testDirectory.Directory, "nested"));
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "file3.txt"), "test");
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "file4.txt"), "test");
                Directory.CreateDirectory(Path.Combine(testDirectory.Directory, "nested", "nested2"));
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "nested2", "file5.txt"), "test");
                
                var webBuilder = new Mock<IWebBuilder>();
                var options = new RegisterOptions();
                options.Matcher = new Matcher();
                options.Matcher.AddInclude("**/file3.*");
                
                webBuilder.Setup(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                
                webBuilder.Object.RegisterDirectory(testDirectory.Directory, options);
                
                webBuilder.Verify(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Never);
                webBuilder.Verify(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Never);
                webBuilder.Verify(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
                webBuilder.Verify(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Never);
                webBuilder.Verify(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Never);
            }
        }
        
        [Fact]
        public void Can_register_files_with_exclude()
        {
            using (var testDirectory = new WorkingDirectorySession())
            {
                File.WriteAllText(Path.Combine(testDirectory.Directory, "file1.txt"), "test");
                File.WriteAllText(Path.Combine(testDirectory.Directory, "file2.txt"), "test");
                Directory.CreateDirectory(Path.Combine(testDirectory.Directory, "nested"));
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "file3.txt"), "test");
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "file4.txt"), "test");
                Directory.CreateDirectory(Path.Combine(testDirectory.Directory, "nested", "nested2"));
                File.WriteAllText(Path.Combine(testDirectory.Directory, "nested", "nested2", "file5.txt"), "test");
                
                var webBuilder = new Mock<IWebBuilder>();
                var options = new RegisterOptions();
                options.Matcher = new Matcher();
                options.Matcher.AddInclude("**/*");
                options.Matcher.AddExclude("**/file3.*");
                
                webBuilder.Setup(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                webBuilder.Setup(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>(), null));
                
                webBuilder.Object.RegisterDirectory(testDirectory.Directory, options);
                
                webBuilder.Verify(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
                webBuilder.Verify(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
                webBuilder.Verify(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Never);
                webBuilder.Verify(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
                webBuilder.Verify(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>(), null), Times.Exactly(1));
            }
        }
        
        [Fact]
        public async Task Can_serve_files()
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
        public async Task Can_serve_files_at_path()
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