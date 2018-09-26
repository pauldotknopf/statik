using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Statik
{
    public class StatikHelpers
    {
        public static string ConvertStringToSlug(string value)
        {
            return Regex.Replace(value, @"[^A-Za-z0-9_\.~]+", "-");
        }
        
        public static string ResolvePathPart(string path, string relative)
        {
            // Preserve the ending query and segment to append it to the result.
            string queryAndSegment = null;
            {
                var index = Math.Min(
                    relative.IndexOf("#", StringComparison.OrdinalIgnoreCase),
                    relative.IndexOf("?", StringComparison.InvariantCultureIgnoreCase));
                if (index > -1)
                {
                    queryAndSegment = relative.Substring(index);
                    relative = relative.Substring(0, index);
                }
            }

            if (relative.StartsWith("/"))
                path = "/";
            
            if (path.Contains("?"))
                path = path.Substring(0, path.IndexOf("?", StringComparison.OrdinalIgnoreCase));

            var fullPathParts = new PathString(path.StartsWith("/") ? path : $"/{path}")
                .Add(relative.StartsWith("/") ? relative : $"/{relative}")
                .Value.Split('/')
                // First entry is empty, because path starts with "/".
                .Skip(1)
                .ToList();

            var stack = new List<string>();

            foreach (var part in fullPathParts)
            {
                if (part == "..")
                {
                    if(stack.Count == 0)
                        throw new Exception($"Invalid path '{relative}'");
                    stack.RemoveAt(stack.Count - 1);
                }else if (part == ".")
                {
                    // Do nothing
                }
                else
                {
                    stack.Add(part);
                }
            }

            return $"/{string.Join("/", stack.ToArray())}{queryAndSegment}";
        }
    }
}