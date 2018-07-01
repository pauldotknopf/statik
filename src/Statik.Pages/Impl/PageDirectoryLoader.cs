using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Statik.Pages.Impl
{
    public class PageDirectoryLoader : IPageDirectoryLoader
    {
        public TreeItem<IFileInfo> LoadFiles(IFileProvider fileProvider, Matcher pageMatcher, Matcher indexMatcher)
        {
            return LoadDirectory(fileProvider, "/", null, pageMatcher, indexMatcher);
        }

        private TreeItem<IFileInfo> LoadDirectory(IFileProvider fileProvider, string basePath, IFileInfo parentDirectory, Matcher pageMatcher, Matcher indexMatcher)
        {
            TreeItem<IFileInfo> root = null;
            var files = fileProvider.GetDirectoryContents(basePath)
                .ToList();

            var children = new List<TreeItem<IFileInfo>>();

            // Load all the files
            foreach (var file in files.Where(x => !x.IsDirectory))
            {
                if (indexMatcher.Match(file.Name).HasMatches)
                {
                    // This is the index page.
                    root = new TreeItem<IFileInfo>(file, basePath);
                }
                else if (pageMatcher.Match(file.Name).HasMatches)
                {
                    children.Add(new TreeItem<IFileInfo>(file, basePath));
                }
            }
            
            if(root == null)
                root = new TreeItem<IFileInfo>(parentDirectory, basePath);
            
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