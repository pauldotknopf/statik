using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Statik.Web;

namespace Statik.Hosting
{
    public interface IHostModule
    {
        void Configure(IApplicationBuilder app, IWebHostEnvironment env);

        void ConfigureServices(IServiceCollection services);

        IReadOnlyCollection<Page> Pages { get; }
    }
}