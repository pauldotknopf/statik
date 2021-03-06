﻿using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Statik.Mvc;
using Statik.Pages;

namespace Statik.Examples.Pages
{
    class Program
    {
        static void Main(string[] args)
        {
            var webBuilder = Statik.GetWebBuilder();
            webBuilder.RegisterMvcServices();
            
            var pagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "pages");
            
            var rootTreeItem = Statik.GetPageDirectoryLoader().LoadFiles(new PhysicalFileProvider(pagesDirectory),
                "*.md",
                "index.md");

            void RegisterTreeItem(PageTreeItem<IFileInfo> treeItem)
            {
                if (!treeItem.Data.IsDirectory)
                {
                    webBuilder.RegisterMvc(treeItem.Path,
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
