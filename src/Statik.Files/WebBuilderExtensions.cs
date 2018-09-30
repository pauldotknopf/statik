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
using Microsoft.Extensions.FileSystemGlobbing;
using Statik.Web;

namespace Statik.Files
{
    public static class WebBuilderExtensions
    {
        public static void RegisterDirectory(this IWebBuilder webBuilder, string directory, RegisterOptions options = null)
        {
            RegisterFileProvider(webBuilder, new PhysicalFileProvider(directory), options);
        }
        
        public static void RegisterDirectory(this IWebBuilder webBuilder, PathString prefix, string directory, RegisterOptions options = null)
        {
            RegisterFileProvider(webBuilder, prefix, new PhysicalFileProvider(directory), options);
        }
        
        public static void RegisterFileProvider(this IWebBuilder webBuilder, IFileProvider fileProvider, RegisterOptions options = null)
        {
            RegisterFileProvider(webBuilder, "/", fileProvider, options);
        }
        
        public static void RegisterFileProvider(this IWebBuilder webBuilder, PathString prefix, IFileProvider fileProvider, RegisterOptions options = null)
        {
            var contents = fileProvider.GetDirectoryContents("/");
            if(contents == null || !contents.Exists) return;

            foreach(var file in contents)
            {
                webBuilder.RegisterFileInfo(fileProvider, prefix, "/", file, options);
            }
        }

        private static void RegisterFileInfo(this IWebBuilder webBuilder, IFileProvider fileProvider, PathString prefix, string basePath, IFileInfo fileInfo, RegisterOptions options)
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
                    webBuilder.RegisterFileInfo(fileProvider, prefix, Path.Combine(basePath, fileInfo.Name), child, options);
                }
            }
            else
            {
                var filePath = Path.Combine(basePath, fileInfo.Name);
                var requestPath = new PathString().Add(prefix)
                    .Add(filePath);

                if (options != null && options.Matcher != null)
                {
                    if (!options.Matcher.Match(filePath.Substring(1)).HasMatches)
                    {
                        // We are ignoring this file
                        return;
                    }
                }
                
                var builtState = options?.State?.Invoke(prefix, requestPath, filePath, fileInfo, fileProvider);
                
                webBuilder.Register(requestPath, async context =>
                    {
                        var env = context.RequestServices.GetRequiredService<IHostingEnvironment>();
    
                        var statileFileOptions = Options.Create(new StaticFileOptions());
                        statileFileOptions.Value.FileProvider = fileProvider;
    
                        var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                        var middleware = new StaticFileMiddleware(_ => Task.CompletedTask, env, statileFileOptions, loggerFactory);
    
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
                    builtState,
                    /*don't convert "/file" to "/file/index.html"*/
                    true);
            }
        }
    }
}