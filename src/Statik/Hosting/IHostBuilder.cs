using Microsoft.AspNetCore.Http;

namespace Statik.Hosting
{
    public interface IHostBuilder
    {
        IWebHost BuildWebHost(int port, PathString appBase, params IHostModule[] modules);

        IVirtualHost BuildVirtualHost(PathString appBase, params IHostModule[] modules);
    }
}