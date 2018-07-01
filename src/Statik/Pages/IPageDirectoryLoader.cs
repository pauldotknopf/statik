using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Statik.Pages
{
    public interface IPageDirectoryLoader
    {
        PageTreeItem<IFileInfo> LoadFiles(IFileProvider fileProvider, Matcher pageMatcher, Matcher indexMatcher);
    }
}