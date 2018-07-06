using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Statik.Pages
{
    public class PageDirectoryLoaderOptions
    {
        public Matcher IndexPageMatcher { get; set; }
        
        public Matcher NormalPageMatcher { get; set; }
        
        public Func<IFileInfo, string> PageSlug { get; set; }

        internal PageDirectoryLoaderOptions Clone()
        {
            return new PageDirectoryLoaderOptions
            {
                IndexPageMatcher = IndexPageMatcher,
                NormalPageMatcher = NormalPageMatcher,
                PageSlug = PageSlug
            };
        }
    }
}