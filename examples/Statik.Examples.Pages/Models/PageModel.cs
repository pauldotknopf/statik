using Microsoft.Extensions.FileProviders;
using Statik.Pages;

namespace Statik.Examples.Pages.Models
{
    public class PageModel
    {
        public PageModel(TreeItem<IFileInfo> treeItem, string content)
        {
            TreeItem = treeItem;
            Content = content;
        }
        
        public TreeItem<IFileInfo> TreeItem { get; }
        
        public string Content { get; }
    }
}