using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Statik.Pages
{
    public static class PageDirectoryLoaderExtensions
    {
        public static PageTreeItem<IFileInfo> LoadFiles(this IPageDirectoryLoader pageDirectoryLoader,
            IFileProvider fileProvider,
            string pageGlob,
            string indexGlob)
        {
            var pageMatcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            pageMatcher.AddInclude(pageGlob);
            var indexMatcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            indexMatcher.AddInclude(indexGlob);
            return pageDirectoryLoader.LoadFiles(fileProvider, new PageDirectoryLoaderOptions
            {
                NormalPageMatcher = pageMatcher,
                IndexPageMatcher = indexMatcher
            });
        }
    }
}