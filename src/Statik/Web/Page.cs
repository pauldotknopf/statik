﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Statik.Web
{
    public class Page
    {
        public Page(string path, Func<HttpContext, Task> action, object state)
        {
            Path = path;
            Action = action;
            State = state;
        }

        public string Path { get; }
            
        public Func<HttpContext, Task> Action { get; }
            
        public object State { get; }
    }
}