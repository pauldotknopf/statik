using System.Collections.Generic;

namespace Statik.Pages
{
    public class PageTreeItem<T>
    {
        public PageTreeItem(T data, string path, bool isIndex)
        {
            Data = data;
            Path = path;
            IsIndex = isIndex;
            Children = new List<PageTreeItem<T>>();
        }

        public T Data { get; }
        
        public string Path { get; }
        
        public bool IsIndex { get; }
        
        public List<PageTreeItem<T>> Children { get; }
    }
}