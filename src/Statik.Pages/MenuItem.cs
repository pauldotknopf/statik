using System.Collections.Generic;

namespace Statik.Pages
{
    public class MenuItem<T>
    {
        public MenuItem(T data)
        {
            Data = data;
            Children = new List<MenuItem<T>>();
        }
        
        public T Data { get; }
        
        public List<MenuItem<T>> Children { get; }
    }
}