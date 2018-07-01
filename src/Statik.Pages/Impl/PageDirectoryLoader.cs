using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Statik.Pages.Impl
{
    public class PageDirectoryLoader : IPageDirectoryLoader
    {
        public MenuItem<IFileInfo> LoadFiles(IFileProvider fileProvider, Matcher pageMatcher, Matcher indexMatcher)
        {
            MenuItem<IFileInfo> root = null;
            var files = fileProvider.GetDirectoryContents("/")
                .ToList();

            var children = new List<MenuItem<IFileInfo>>();

            foreach (var file in files.Where(x => !x.IsDirectory))
            {
                if (indexMatcher.Match(file.Name).HasMatches)
                {
                    // This is the index page.
                    root = new MenuItem<IFileInfo>(file);
                } else if (pageMatcher.Match(file.Name).HasMatches)
                {
                    children.Add(new MenuItem<IFileInfo>(file));
                }
            }
            
            if(root == null)
                root = new MenuItem<IFileInfo>(null);
            
            root.Children.AddRange(children);

            return root;
        }
    }
}