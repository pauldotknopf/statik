using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Statik.Hosting;

namespace Statik.Web
{
    public interface IWebBuilder
    {
        void Register(string path, Func<HttpContext, Task> action, object state = null);

        void RegisterServices(Action<IServiceCollection> action);
        
        IWebHost BuildWebHost(string appBase = null, int port = StatikDefaults.DefaultPort);

        IVirtualHost BuildVirtualHost(string appBase = null);
    }
}