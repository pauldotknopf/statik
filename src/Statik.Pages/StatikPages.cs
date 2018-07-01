using System;
using Microsoft.Extensions.DependencyInjection;
using Statik.Web;

namespace Statik.Pages
{
    public class StatikPages
    {
        private static readonly object Lock = new object();
        private static IServiceProvider _serviceProvider;
        
        public static IPageDirectoryLoader GetPageDirectoryLoader()
        {
            EnsureServiceProvider();
            return _serviceProvider.GetRequiredService<IPageDirectoryLoader>();
        }

        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IPageDirectoryLoader, Impl.PageDirectoryLoader>();
        }

        private static void EnsureServiceProvider()
        {
            if (_serviceProvider != null) return;
            lock (Lock)
            {
                if (_serviceProvider != null) return;
                
                var services = new ServiceCollection();
                
                Statik.RegisterServices(services);
                RegisterServices(services);

                _serviceProvider = services.BuildServiceProvider();
            }
        }
    }
}