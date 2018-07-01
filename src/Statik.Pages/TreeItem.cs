using System.Collections.Generic;

namespace Statik.Pages
{
    public class TreeItem<T>
    {
        public TreeItem(T data, string path)
        {
            Data = data;
            Path = path;
            Children = new List<TreeItem<T>>();
        }

        public T Data { get; }
        
        public string Path { get; }
        
        public List<TreeItem<T>> Children { get; }
    }
}