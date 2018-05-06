using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Statik.Web;

namespace Statik.Files
{
    public static class WebBuilderExtensions
    {
        public static void RegisterFiles(this IWebBuilder webBuilder, IFileProvider fileProvider)
        {
            RegisterFiles(webBuilder, "/", fileProvider);
        }

        public static void RegisterFiles(this IWebBuilder webBuilder, PathString prefix, IFileProvider fileProvider)
        {
            var contents = fileProvider.GetDirectoryContents("/");
            if(contents == null || !contents.Exists) return;

            foreach(var file in contents)
            {
                webBuilder.RegisterFileInfo(fileProvider, prefix, "/", file);
            }
        }

        private static void RegisterFileInfo(this IWebBuilder webBuilder, IFileProvider fileProvider, PathString prefix,
            string basePath, IFileInfo fileInfo)
        {
            if (fileInfo.IsDirectory)
            {
                var content = fileProvider.GetDirectoryContents(fileInfo.Name);

                if (content == null || !content.Exists)
                {
                    return;
                }

                foreach (var child in content)
                {
                    webBuilder.RegisterFileInfo(fileProvider, prefix, Path.Combine(basePath, fileInfo.Name), child);
                }
            }
            else
            {
                var path = new PathString().Add(prefix)
                    .Add(basePath)
                    .Add("/" + fileInfo.Name);

                webBuilder.Register(path.Value, async context =>
                {
                    var env = context.RequestServices.GetRequiredService<IHostingEnvironment>();

                    var options = Options.Create(new StaticFileOptions());
                    options.Value.FileProvider = fileProvider;

                    var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                    var middleware = new StaticFileMiddleware(_ => Task.CompletedTask, env, options, loggerFactory);

                    var oldPath = context.Request.Path;
                    try
                    {
                        context.Request.Path = Path.Combine(basePath, fileInfo.Name);
                        await middleware.Invoke(context);
                    }
                    finally
                    {
                        context.Request.Path = oldPath;
                    }
                });
            }
        }
    }
}