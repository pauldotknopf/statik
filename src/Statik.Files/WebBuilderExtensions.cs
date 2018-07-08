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
        public static void RegisterDirectory(this IWebBuilder webBuilder, string directory, BuildStateDelegate state = null)
        {
            RegisterFileProvider(webBuilder, new PhysicalFileProvider(directory));
        }
        
        public static void RegisterDirectory(this IWebBuilder webBuilder, PathString prefix, string directory, BuildStateDelegate state = null)
        {
            RegisterFileProvider(webBuilder, prefix, new PhysicalFileProvider(directory));
        }
        
        public static void RegisterFileProvider(this IWebBuilder webBuilder, IFileProvider fileProvider, BuildStateDelegate state = null)
        {
            RegisterFileProvider(webBuilder, "/", fileProvider);
        }
        
        public static void RegisterFileProvider(this IWebBuilder webBuilder, PathString prefix, IFileProvider fileProvider, BuildStateDelegate state = null)
        {
            var contents = fileProvider.GetDirectoryContents("/");
            if(contents == null || !contents.Exists) return;

            foreach(var file in contents)
            {
                webBuilder.RegisterFileInfo(fileProvider, prefix, "/", file);
            }
        }

        private static void RegisterFileInfo(this IWebBuilder webBuilder, IFileProvider fileProvider, PathString prefix, string basePath, IFileInfo fileInfo, BuildStateDelegate state = null)
        {
            if (fileInfo.IsDirectory)
            {
                var content = fileProvider.GetDirectoryContents(Path.Combine(basePath, fileInfo.Name));

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
                var filePath = Path.Combine(basePath, fileInfo.Name);
                var requestPath = new PathString().Add(prefix)
                    .Add(filePath);

                var builtState = state?.Invoke(prefix, requestPath, filePath, fileInfo, fileProvider);
                
                webBuilder.Register(requestPath, async context =>
                    {
                        var env = context.RequestServices.GetRequiredService<IHostingEnvironment>();
    
                        var options = Options.Create(new StaticFileOptions());
                        options.Value.FileProvider = fileProvider;
    
                        var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                        var middleware = new StaticFileMiddleware(_ => Task.CompletedTask, env, options, loggerFactory);
    
                        var oldPath = context.Request.Path;
                        try
                        {
                            context.Request.Path = filePath;
                            await middleware.Invoke(context);
                        }
                        finally
                        {
                            context.Request.Path = oldPath;
                        }
                    },
                    builtState);
            }
        }
        
        public delegate object BuildStateDelegate(string requestPrefix, string requestFullPath, string filePath, IFileInfo file, IFileProvider fileProvider);
    }
}