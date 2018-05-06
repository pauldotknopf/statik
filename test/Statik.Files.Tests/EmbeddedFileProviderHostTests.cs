using System.Threading.Tasks;
using Statik.Embedded;
using Statik.Hosting.Impl;
using Statik.Tests;
using Statik.Web.Impl;
using Xunit;

namespace Statik.Files.Tests
{
    public class EmbeddedFileProviderHostTests
    {
        [Fact]
        public async Task Can_host_embedded_fies()
        {
            var embeddedFiles = new EmbeddedFileProvider(typeof(EmbeddedFileProviderTests).Assembly, "Statik.Tests.Embedded");
            var webBuilder = new WebBuilder(new HostBuilder());
            
            webBuilder.RegisterFileProvider(embeddedFiles);

            using (var host = webBuilder.BuildVirtualHost())
            {
                using (var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/file1.txt");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();
                    
                    Assert.Equal("file1content", response);
                    
                    responseMessage = await client.GetAsync("/nested/file2.txt");
                    responseMessage.EnsureSuccessStatusCode();
                    response = await responseMessage.Content.ReadAsStringAsync();
                    
                    Assert.Equal("file2content", response);
                }
            }
        }
    }
}