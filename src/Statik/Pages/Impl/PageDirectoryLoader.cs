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
        public PageTreeItem<IFileInfo> LoadFiles(IFileProvider fileProvider, Matcher pageMatcher, Matcher indexMatcher)
        {
            return LoadDirectory(fileProvider, "", null, pageMatcher, indexMatcher);
        }

        private PageTreeItem<IFileInfo> LoadDirectory(IFileProvider fileProvider, string basePath, IFileInfo parentDirectory, Matcher pageMatcher, Matcher indexMatcher)
        {
            PageTreeItem<IFileInfo> root = null;
            var files = fileProvider.GetDirectoryContents(basePath == "" ? "/" : basePath)
                .ToList();

            var children = new List<PageTreeItem<IFileInfo>>();

            // Load all the files
            foreach (var file in files.Where(x => !x.IsDirectory))
            {
                if (indexMatcher.Match(file.Name).HasMatches)
                {
                    // This is the index page.
                    root = new PageTreeItem<IFileInfo>(file, basePath, true);
                }
                else if (pageMatcher.Match(file.Name).HasMatches)
                {
                    children.Add(new PageTreeItem<IFileInfo>(file, basePath, false));
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
                var treeItem = LoadDirectory(fileProvider, path, directory, pageMatcher, indexMatcher);
                if (treeItem != null)
                {
                    root.Children.Add(treeItem);
                }
            }

            return root;
        }
    }
}