using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Statik.Hosting
{
    public interface IHost : IDisposable
    {
        IReadOnlyCollection<string> Paths { get; }

        HttpClient CreateClient();

        IServiceProvider ServiceProvider { get; }
    }
}