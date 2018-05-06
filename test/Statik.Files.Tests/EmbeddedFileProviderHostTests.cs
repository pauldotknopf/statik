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
        public void Can_host_embedded_fies()
        {
            var embeddedFiles = new EmbeddedFileProvider(typeof(EmbeddedFileProviderTests).Assembly, "Statik.Tests.Embedded");
            var webBuilder = new Mock<IWebBuilder>();

            webBuilder.Setup(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>()));
            webBuilder.Setup(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>()));
            webBuilder.Setup(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>()));
            webBuilder.Setup(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>()));
            webBuilder.Setup(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>()));
            
            webBuilder.Object.RegisterFileProvider(embeddedFiles);
            
            webBuilder.Verify(x => x.Register("/file1.txt", It.IsAny<Func<HttpContext, Task>>()), Times.Exactly(1));
            webBuilder.Verify(x => x.Register("/file2.txt", It.IsAny<Func<HttpContext, Task>>()), Times.Exactly(1));
            webBuilder.Verify(x => x.Register("/nested/file3.txt", It.IsAny<Func<HttpContext, Task>>()), Times.Exactly(1));
            webBuilder.Verify(x => x.Register("/nested/file4.txt", It.IsAny<Func<HttpContext, Task>>()), Times.Exactly(1));
            webBuilder.Verify(x => x.Register("/nested/nested2/file5.txt", It.IsAny<Func<HttpContext, Task>>()), Times.Exactly(1));
        }
    }
}