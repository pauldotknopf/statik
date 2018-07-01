using Microsoft.Extensions.DependencyInjection;

namespace Statik.Pages
{
    public class StatikPages
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IPageDirectoryLoader, Impl.PageDirectoryLoader>();
        }
    }
}