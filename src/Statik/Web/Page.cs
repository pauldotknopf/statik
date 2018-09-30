using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Statik.Web
{
    public class Page
    {
        internal Page(string path,
            Func<HttpContext, Task> action,
            object state,
            bool extractExactPath)
        {
            Path = path;
            Action = action;
            State = state;
            ExtractExactPath = extractExactPath;
        }

        public string Path { get; }
            
        internal Func<HttpContext, Task> Action { get; }
        
        public object State { get; }
        
        public bool ExtractExactPath { get; }
    }
}