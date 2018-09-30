using System;
using System.Threading.Tasks;
using Statik.Mvc;

namespace Statik.Examples.Mvc
{
    class Program
    {
        static void Main(string[] args)
        {
            var webBuilder = Statik.GetWebBuilder();
            
            webBuilder.RegisterMvcServices();
            webBuilder.RegisterMvc("/hello", new
            {
                controller = "Example",
                action = "Index",
                other = 1 /* You can pass other values to the action, traditional MVC */
            });

            using (var host = webBuilder.BuildWebHost())
            {
                host.Listen();
                Console.WriteLine($"Listening on port {StatikDefaults.DefaultPort}...");
                Console.WriteLine($"Try visiting http://localhost:{StatikDefaults.DefaultPort}/hello");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                
                // NOTE: You can export this host to a directory.
                // Useful for GitHub pages, etc.
                // await Statik.ExportHost(host, "./somewhere");
            }
        }
    }
}