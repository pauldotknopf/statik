using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Statik.Mvc;
using Statik.Pages;

namespace Statik.Examples.Pages
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var webBuilder = Statik.GetWebBuilder();
            webBuilder.RegisterMvcServices();
            
            var pagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "pages");
            
            var rootTreeItem = StatikPages.GetPageDirectoryLoader().LoadFiles(new PhysicalFileProvider(pagesDirectory),
                "*.md",
                "index.md");

            Console.Write(JsonConvert.SerializeObject(rootTreeItem, Formatting.Indented));
            
            void RegisterTreeItem(PageTreeItem<IFileInfo> treeItem)
            {
                if (!treeItem.Data.IsDirectory)
                {
                    webBuilder.RegisterMvc(
                        $"{treeItem.BasePath}/{Path.GetFileNameWithoutExtension(treeItem.Data.Name)}",
                        new
                        {
                            controller = "Pages",
                            action = "Index",
                            treeItem
                        });
                }
                
                foreach (var child in treeItem.Children)
                {
                    RegisterTreeItem(child);
                }
            }
            
            RegisterTreeItem(rootTreeItem);
            
            using (var host = webBuilder.BuildWebHost())
            {
                host.Listen();
                Console.WriteLine($"Listening on port {StatikDefaults.DefaultPort}...");
                Console.WriteLine($"Try visiting http://localhost:{StatikDefaults.DefaultPort}/hello");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                
            }
        }
    }
}
