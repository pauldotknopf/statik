using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Statik.Pages
{
    public interface IPageDirectoryLoader
    {
        TreeItem<IFileInfo> LoadFiles(IFileProvider fileProvider, Matcher pageMatcher, Matcher indexMatcher);
    }
}