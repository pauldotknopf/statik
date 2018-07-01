using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Statik.Pages.Impl
{
    public class PageDirectoryLoader : IPageDirectoryLoader
    {
        public MenuItem<IFileInfo> LoadFiles(IFileProvider fileProvider, Matcher pageMatcher, Matcher indexMatcher)
        {
            return LoadDirectory(fileProvider, "/", null, pageMatcher, indexMatcher);
        }

        private MenuItem<IFileInfo> LoadDirectory(IFileProvider fileProvider, string basePath, IFileInfo parentDirectory, Matcher pageMatcher, Matcher indexMatcher)
        {
            MenuItem<IFileInfo> root = null;
            var files = fileProvider.GetDirectoryContents(basePath)
                .ToList();

            var children = new List<MenuItem<IFileInfo>>();

            // Load all the files
            foreach (var file in files.Where(x => !x.IsDirectory))
            {
                if (indexMatcher.Match(file.Name).HasMatches)
                {
                    // This is the index page.
                    root = new MenuItem<IFileInfo>(file);
                }
                else if (pageMatcher.Match(file.Name).HasMatches)
                {
                    children.Add(new MenuItem<IFileInfo>(file));
                }
            }
            
            if(root == null)
                root = new MenuItem<IFileInfo>(parentDirectory);
            
            root.Children.AddRange(children);
            
            // Load all the child directories
            foreach (var directory in files.Where(x => x.IsDirectory))
            {
                var path = new PathString().Add(basePath)
                    .Add("/" + directory.Name);
                var menuItem = LoadDirectory(fileProvider, path, directory, pageMatcher, indexMatcher);
                if (menuItem != null)
                {
                    root.Children.Add(menuItem);
                }
            }

            return root;
        }
    }
}