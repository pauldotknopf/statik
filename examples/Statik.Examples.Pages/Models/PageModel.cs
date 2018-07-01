using Microsoft.Extensions.FileProviders;
using Statik.Pages;

namespace Statik.Examples.Pages.Models
{
    public class PageModel
    {
        public PageModel(PageTreeItem<IFileInfo> treeItem, string content)
        {
            TreeItem = treeItem;
            Content = content;
        }
        
        public PageTreeItem<IFileInfo> TreeItem { get; }
        
        public string Content { get; }
    }
}