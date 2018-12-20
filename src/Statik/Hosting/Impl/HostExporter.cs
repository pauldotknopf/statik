using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Statik.Web;

namespace Statik.Hosting.Impl
{
    public class HostExporter : IHostExporter
    {
        public async Task Export(IHost host, string destinationDirectory)
        {
            await PrepareDirectory(destinationDirectory);

            var context = new SemaphoreSlim(1, 1);
            
            foreach(var page in host.Pages)
            {
                using(var client = host.CreateClient())
                {
                    var destination = $"{destinationDirectory}{page.Path}";
                    if (!page.ExtractExactPath)
                    {
                        if (string.IsNullOrEmpty(Path.GetExtension(destination)))
                        {
                            destination += "/index.html";
                        }
                    }
                    await SaveUrlToFile(client, page, destination, context);
                }
            }
        }

        public async Task ExportParallel(IHost host, string destinationDirectory, int? maxThreads = null)
        {
            await PrepareDirectory(destinationDirectory);

            if (!maxThreads.HasValue)
            {
                maxThreads = Environment.ProcessorCount;
            }

            if (maxThreads < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxThreads));
            }
            
            var context = new SemaphoreSlim(1, 1);
            
            var exportPage = new ActionBlock<Page>(async page =>
            {
                using(var client = host.CreateClient())
                {
                    var destination = $"{destinationDirectory}{page.Path}";
                    if (!page.ExtractExactPath)
                    {
                        if (string.IsNullOrEmpty(Path.GetExtension(destination)))
                        {
                            destination += "/index.html";
                        }
                    }
                    await SaveUrlToFile(client, page, destination, context);
                }
            },  new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxThreads.Value
            });

            foreach (var page in host.Pages)
            {
                await exportPage.SendAsync(page);
            }

            exportPage.Complete();

            await exportPage.Completion;
        }

        private async Task PrepareDirectory(string directory)
        {
            if(!await Task.Run(() => Directory.Exists(directory)))
            {
                await Task.Run(() => Directory.CreateDirectory(directory));
            }

            // Clean all the files currently in the directory
            foreach(var fileToDelete in await Task.Run(() => Directory.GetFiles(directory)))
            {
                await Task.Run(() => File.Delete(fileToDelete));
            }
            foreach(var directoryToDelete in await Task.Run(() => Directory.GetDirectories(directory)))
            {
                await Task.Run(() => Directory.Delete(directoryToDelete, true));
            }
        }

        private async Task SaveUrlToFile(HttpClient client, Page page, string file, SemaphoreSlim context)
        {
            // Ensure the file's parent directories are created.
            var parentDirectory = Path.GetDirectoryName(file);

            // Lock here to prevent multiple threads from creating the same directory.
            await context.WaitAsync();
            try
            {
                if (!string.IsNullOrEmpty(parentDirectory))
                {
                    if (!(await Task.Run(() => Directory.Exists(parentDirectory))))
                    {
                        await Task.Run(() => Directory.CreateDirectory(parentDirectory));
                    }
                }
            }
            finally
            {
                context.Release();
            }
            
            var response = await client.GetAsync(page.Path);
            response.EnsureSuccessStatusCode();
            using(var requestStream = await response.Content.ReadAsStreamAsync())
            {
                using(var fileStream = await Task.Run(() => File.OpenWrite(file)))
                    await requestStream.CopyToAsync(fileStream);
            }
        }
    }
}