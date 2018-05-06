using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Statik.Hosting;
using Statik.Hosting.Impl;
using Xunit;

namespace Statik.Tests
{
    public class HostingTests
    {
        readonly IHostBuilder _hostBuilder;

        public HostingTests()
        {
            _hostBuilder = new HostBuilder();
        }

        [Fact]
        public async Task Can_create_web_host()
        {
            using(var host = _hostBuilder.BuildWebHost(5002, "", BuildTestHostModule()))
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
        }

        [Fact]
        public async Task Can_create_virtual_host()
        {
            using(var host = _hostBuilder.BuildVirtualHost("", BuildTestHostModule()))
            {
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test ", response);
                }
            }
        }
        
        private IHostModule BuildTestHostModule()
        {
            var hostModule = new Mock<IHostModule>();
            hostModule.Setup(x => x.Configure(It.IsAny<IApplicationBuilder>(), It.IsAny<IHostingEnvironment>()))
                .Callback((IApplicationBuilder app, IHostingEnvironment env) => {
                    app.Run(async context => {
                        await context.Response.WriteAsync("Hello, World! " + context.Request.Path + " " + context.Request.PathBase);
                    });
                });
            hostModule.Setup(x => x.ConfigureServices(It.IsAny<IServiceCollection>()));
            hostModule.Setup(x => x.Paths).Returns(new ReadOnlyCollection<string>(new List<string>{ "/test" }));
            return hostModule.Object;
        }
    }
}