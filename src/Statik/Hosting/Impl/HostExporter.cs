﻿using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Statik.Web;

namespace Statik.Hosting.Impl
{
    public class HostExporter : IHostExporter
    {
        public async Task Export(IHost host, string destinationDirectory)
        {
            if(!await Task.Run(() => Directory.Exists(destinationDirectory)))
            {
                await Task.Run(() => Directory.CreateDirectory(destinationDirectory));
            }

            // Clean all the files currently in the directory
            foreach(var fileToDelete in await Task.Run(() => Directory.GetFiles(destinationDirectory)))
            {
                await Task.Run(() => File.Delete(fileToDelete));
            }
            foreach(var directoryToDelete in await Task.Run(() => Directory.GetDirectories(destinationDirectory)))
            {
                await Task.Run(() => Directory.Delete(directoryToDelete, true));
            }

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
                    await SaveUrlToFile(client, page, destination);
                }
            }
        }

        private async Task SaveUrlToFile(HttpClient client, Page page, string file)
        {
            // Ensure the file's parent directories are created.
            var parentDirectory = Path.GetDirectoryName(file);
            if (!string.IsNullOrEmpty(parentDirectory))
            {
                if (!(await Task.Run(() => Directory.Exists(parentDirectory))))
                {
                    await Task.Run(() => Directory.CreateDirectory(parentDirectory));
                }
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