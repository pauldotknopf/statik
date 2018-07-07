using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Statik.Hosting;
using Statik.Hosting.Impl;
using Statik.Pages;
using Statik.Pages.Impl;
using Statik.Web;
using Statik.Web.Impl;

namespace Statik
{
    public static class Statik
    {
        private static readonly object Lock = new object();
        private static IServiceProvider _serviceProvider;
        
        public static IWebBuilder GetWebBuilder()
        {
            EnsureServiceProvider();
            return _serviceProvider.GetRequiredService<IWebBuilderFactory>().CreateWebBuilder();
        }

        public static IPageDirectoryLoader GetPageDirectoryLoader()
        {
            EnsureServiceProvider();
            return _serviceProvider.GetRequiredService<IPageDirectoryLoader>();
        }

        public static Task ExportHost(IHost host, string directory)
        {
            EnsureServiceProvider();
            return _serviceProvider.GetRequiredService<IHostExporter>().Export(host, directory);
        }

        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IHostBuilder, HostBuilder>();
            services.AddSingleton<IHostExporter, HostExporter>();
            services.AddSingleton<IWebBuilderFactory, WebBuilderFactory>();
            services.AddSingleton<IPageDirectoryLoader, PageDirectoryLoader>();
        }

        private static void EnsureServiceProvider()
        {
            if (_serviceProvider != null) return;
            lock (Lock)
            {
                if (_serviceProvider != null) return;
                
                var services = new ServiceCollection();
                
                RegisterServices(services);

                _serviceProvider = services.BuildServiceProvider();
            }
        }
    }
}