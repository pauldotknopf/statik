using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Statik.Pages
{
    public class PageTreeItem<T>
    {
        public PageTreeItem()
        {
            Children = new List<PageTreeItem<T>>();
        }

        public PageTreeItem(T data, string path, string filePath)
            : this()
        {
            Data = data;
            Path = path;
            FilePath = filePath;
        }

        public T Data { get; set; }
        
        public string Path { get; set; }
        
        public string FilePath { get; set; }
        
        public List<PageTreeItem<T>> Children { get; set; }
        
        public PageTreeItem<T> Parent { get; set; }

        public async Task<PageTreeItem<TNew>> Convert<TNew>(Func<PageTreeItem<T>, Task<TNew>> convert)
        {
            var newTree = new PageTreeItem<TNew>
            {
                Path = Path,
                FilePath = FilePath,
                Data = await convert(this)
            };

            foreach (var child in Children)
            {
                newTree.Children.Add(await child.Convert(convert));
            }

            return newTree;
        }
    }
}