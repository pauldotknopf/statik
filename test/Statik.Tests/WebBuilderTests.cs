using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Statik.Web;
using Xunit;

namespace Statik.Tests
{
    public class WebBuilderTests
    {
        readonly IWebBuilder _webBuilder;

        public WebBuilderTests()
        {
            _webBuilder = new Web.Impl.WebBuilder(new Hosting.Impl.HostBuilder());
        }

        [Fact]
        public async Task Can_use_with_app_base_with_web()
        {
            _webBuilder.Register("/test", async context =>
            {
                await context.Response.WriteAsync("Hello, World! " + context.Request.Path + " " + context.Request.PathBase);
            });
            
            using(var host = _webBuilder.BuildWebHost("", 5003))
            {
                host.Listen();
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test ", response);
                }
            }
            
            using(var host = _webBuilder.BuildWebHost("/appbase", 5003))
            {
                host.Listen();
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test /appbase", response);
                }
            }
        }
        
        [Fact]
        public async Task Can_use_with_app_base_with_virtual()
        {
            _webBuilder.Register("/test", async context =>
            {
                await context.Response.WriteAsync("Hello, World! " + context.Request.Path + " " + context.Request.PathBase);
            });
            
            using(var host = _webBuilder.BuildVirtualHost(""))
            {
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test ", response);
                }
            }
            
            using(var host = _webBuilder.BuildVirtualHost("/appbase"))
            {
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test /appbase", response);
                }
            }
        }
    }
}