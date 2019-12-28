using Microsoft.Extensions.FileProviders;

namespace Statik.Pages
{
    public interface IPageDirectoryLoader
    {
        PageTreeItem<IFileInfo> LoadFiles(IFileProvider fileProvider, PageDirectoryLoaderOptions options);
    }
}