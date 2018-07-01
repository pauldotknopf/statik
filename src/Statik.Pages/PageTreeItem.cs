using System.Collections.Generic;

namespace Statik.Pages
{
    public class PageTreeItem<T>
    {
        public PageTreeItem(T data, string basePath, bool isIndex)
        {
            Data = data;
            BasePath = basePath;
            IsIndex = isIndex;
            Children = new List<PageTreeItem<T>>();
        }

        public T Data { get; }
        
        public string BasePath { get; }
        
        public bool IsIndex { get; }
        
        public List<PageTreeItem<T>> Children { get; }
    }
}