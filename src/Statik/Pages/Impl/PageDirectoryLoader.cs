using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Statik.Pages.Impl
{
    public class PageDirectoryLoader : IPageDirectoryLoader
    {
        public PageTreeItem<IFileInfo> LoadFiles(IFileProvider fileProvider, PageDirectoryLoaderOptions options)
        {
            if(options == null) throw new ArgumentNullException(nameof(options));
            options = options.Clone();
            if(options.PageSlug == null)
                options.PageSlug = fileInfo => StatikHelpers.ConvertStringToSlug(Path.GetFileNameWithoutExtension(fileInfo.Name));
            return LoadDirectory(fileProvider, "/", null, options);
        }

        private PageTreeItem<IFileInfo> LoadDirectory(IFileProvider fileProvider, string basePath, IFileInfo parentDirectory, PageDirectoryLoaderOptions options)
        {
            PageTreeItem<IFileInfo> root = null;
            var files = fileProvider.GetDirectoryContents(basePath)
                .ToList();

            var children = new List<PageTreeItem<IFileInfo>>();

            // Load all the files
            foreach (var file in files.Where(x => !x.IsDirectory))
            {
                if (options.IndexPageMatcher != null && options.IndexPageMatcher.Match(file.Name).HasMatches)
                {
                    // This is the index page.
                    root = new PageTreeItem<IFileInfo>(file, basePath, true);
                }
                else
                {
                    var path = basePath == "/"
                        ? $"/{options.PageSlug(file)}"
                        : $"{basePath}/{options.PageSlug(file)}";
                    
                    if (options.NormalPageMatcher != null)
                    {
                        if (options.NormalPageMatcher.Match(file.Name).HasMatches)
                        {
                            children.Add(new PageTreeItem<IFileInfo>(file, path, false));
                        }
                    }
                    else
                    {
                        children.Add(new PageTreeItem<IFileInfo>(file, path, false));
                    }
                }
            }
            
            if(root == null)
                root = new PageTreeItem<IFileInfo>(parentDirectory, basePath, false);
            
            root.Children.AddRange(children);
            
            // Load all the child directories
            foreach (var directory in files.Where(x => x.IsDirectory))
            {
                var path = new PathString().Add(basePath)
                    .Add("/" + directory.Name);
                var treeItem = LoadDirectory(fileProvider, path, directory, options);
                if (treeItem != null)
                {
                    root.Children.Add(treeItem);
                }
            }

            foreach (var child in root.Children)
            {
                child.Parent = root;
            }

            return root;
        }
    }
}