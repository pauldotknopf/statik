using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Statik.Web;

namespace Statik.Hosting.Impl
{
    public class HostBuilder : IHostBuilder
    {
        public IWebHost BuildWebHost(int port, PathString appBase, params IHostModule[] modules)
        {
            return new InternalWebHost(appBase, modules.ToList(), port);
        }

        public IVirtualHost BuildVirtualHost(PathString appBase, params IHostModule[] modules)
        {
            return new InternalVirtualHost(appBase, modules.ToList());
        }

        private class InternalWebHost : IWebHost
        {
            readonly Microsoft.AspNetCore.Hosting.IWebHost _webHost;
            readonly PathString _appBase;
            readonly int _port;

            public InternalWebHost(
                PathString appBase,
                List<IHostModule> modules,
                int port)
            {
                _appBase = appBase;
                _port = port;
                Pages = new ReadOnlyCollection<Page>(modules.SelectMany(x => x.Pages).ToList());
                _webHost = WebHost.CreateDefaultBuilder(new string[]{})
                    .UseUrls($"http://*:{port}")
                    .UseSetting(WebHostDefaults.ApplicationKey,  Assembly.GetEntryAssembly().GetName().Name)
                    .ConfigureLogging(factory => {
                        factory.AddConsole();
                    })
                    .ConfigureServices(services => {
                        services.AddSingleton(modules);
                    })
                    .UseStartup<Startup>()
                    .Build();
            }

            public IReadOnlyCollection<Page> Pages { get; }

            public HttpClient CreateClient()
            {
                var inner = new HttpClient() {
                    BaseAddress = new Uri($"http://localhost:{_port}")
                };
                var wrapper = new HttpClient(new AppBaseAppendMessageHandler(inner, _appBase));
                wrapper.BaseAddress = inner.BaseAddress;
                return wrapper;
            }

            public IServiceProvider ServiceProvider => _webHost.Services;

            public void Listen()
            {
                _webHost.Start();
            }

            public void Dispose()
            {
                _webHost.Dispose();
            }
        }

        private class InternalVirtualHost : IVirtualHost
        {
            readonly PathString _appBase;
            readonly TestServer _testServer;

            public InternalVirtualHost(
                PathString appBase,
                List<IHostModule> modules)
            {
                _appBase = appBase;
                Pages = new ReadOnlyCollection<Page>(modules.SelectMany(x => x.Pages).ToList());
                _testServer = new TestServer(new WebHostBuilder()
                    .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly().GetName().Name)
                    .ConfigureLogging(factory => { factory.AddConsole(); })
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton(modules);
                    })
                    .UseStartup<Startup>());
            }

            public IReadOnlyCollection<Page> Pages { get; }

            public HttpClient CreateClient()
            {
                var inner = _testServer.CreateClient();
                var wrapper = new HttpClient(new AppBaseAppendMessageHandler(inner, _appBase));
                wrapper.BaseAddress = inner.BaseAddress;
                return wrapper;
            }

            public IServiceProvider ServiceProvider => _testServer.Host.Services;

            public void Dispose()
            {
                _testServer.Dispose();
            }
        }
    }
}