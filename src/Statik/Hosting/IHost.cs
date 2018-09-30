using System;
using System.Collections.Generic;
using System.Net.Http;
using Statik.Web;

namespace Statik.Hosting
{
    public interface IHost : IDisposable
    {
        IReadOnlyCollection<Page> Pages { get; }

        HttpClient CreateClient();

        IServiceProvider ServiceProvider { get; }
    }
}