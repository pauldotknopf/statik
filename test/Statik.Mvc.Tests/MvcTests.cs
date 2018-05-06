using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Statik.Hosting.Impl;
using Statik.Web;
using Statik.Web.Impl;
using Xunit;

namespace Statik.Mvc.Tests
{
    public class MvcTests
    {
        private IWebBuilder _webBuilder;
        
        public MvcTests()
        {
            _webBuilder = new WebBuilder(new HostBuilder());
            _webBuilder.RegisterMvcServices();
        }
        
        [Fact]
        public async Task Can_call_mvc_action()
        {
            _webBuilder.RegisterMvc("/somewhere", new
            {
                controller = "Test",
                action = "Index"
            });
            using(var host = _webBuilder.BuildVirtualHost())
            {
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/somewhere");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("test", response);
                }
            }
        }
    }
    
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            return Content("test", "application/text");
        }
    }
}