using System.Collections.Generic;

namespace Statik.Pages
{
    public class PageTreeItem<T>
    {
        public PageTreeItem()
        {
            Children = new List<PageTreeItem<T>>();
        }

        public PageTreeItem(T data, string path, bool isIndex)
            : this()
        {
            Data = data;
            Path = path;
            IsIndex = isIndex;
        }

        public T Data { get; set; }
        
        public string Path { get; set; }
        
        public bool IsIndex { get; set; }
        
        public List<PageTreeItem<T>> Children { get; set; }
        
        public PageTreeItem<T> Parent { get; set; }
    }
}