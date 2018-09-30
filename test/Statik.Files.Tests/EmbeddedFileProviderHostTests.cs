using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Statik.Embedded;
using Statik.Hosting.Impl;
using Statik.Tests;
using Statik.Web;
using Statik.Web.Impl;
using Xunit;

namespace Statik.Files.Tests
{
    public class EmbeddedFileProviderHostTests
    {
        [Fact]
        public void Can_register_embedded_files()
        {
            var embeddedFiles = new EmbeddedFileProvider(typeof(EmbeddedFileProviderTests).Assembly, "Statik.Tests.Embedded");
            var webBuilder = new Mock<IWebBuilder>();

            webBuilder.Setup(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>(), null, true));
            webBuilder.Setup(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>(), null, true));
            webBuilder.Setup(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>(), null, true));
            webBuilder.Setup(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>(), null, true));
            webBuilder.Setup(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>(), null, true));
            
            webBuilder.Object.RegisterFileProvider(embeddedFiles);
            
            webBuilder.Verify(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>(), null, true), Times.Exactly(1));
            webBuilder.Verify(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>(), null, true), Times.Exactly(1));
            webBuilder.Verify(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>(), null, true), Times.Exactly(1));
            webBuilder.Verify(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>(), null, true), Times.Exactly(1));
            webBuilder.Verify(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>(), null, true), Times.Exactly(1));
        }

        [Fact]
        public async Task Can_serve_embedded_files()
        {
            var embeddedFiles = new EmbeddedFileProvider(typeof(EmbeddedFileProviderTests).Assembly, "Statik.Tests.Embedded");
            var webBuilder = new WebBuilder(new HostBuilder());
            
            webBuilder.RegisterFileProvider(embeddedFiles);
            
            using(var host = webBuilder.BuildVirtualHost())
            {
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/file1.txt");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("file1content", response);
                    
                    responseMessage = await client.GetAsync("/nested/nested2/file5.txt");
                    responseMessage.EnsureSuccessStatusCode();
                    response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("file5content", response);
                }
            }
            
            using(var host = webBuilder.BuildVirtualHost("/appbase"))
            {
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/file1.txt");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("file1content", response);
                    
                    responseMessage = await client.GetAsync("/nested/nested2/file5.txt");
                    responseMessage.EnsureSuccessStatusCode();
                    response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("file5content", response);
                }
            }
        }
    }
}