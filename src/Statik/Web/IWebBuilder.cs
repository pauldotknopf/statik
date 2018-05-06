using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Statik.Hosting;

namespace Statik.Web
{
    public interface IWebBuilder
    {
        void Register(string path, Func<HttpContext, Task> action);

        void RegisterServices(Action<IServiceCollection> action);
        
        IWebHost BuildWebHost(string appBase = null, int port = 8000);

        IVirtualHost BuildVirtualHost(string appBase = null);
    }
}