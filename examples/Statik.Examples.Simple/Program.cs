using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Statik.Examples.Simple
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var webBuilder = Statik.GetWebBuilder();
            
            webBuilder.Register("/hello", async context =>
            {
                await context.Response.WriteAsync($"The time is {DateTime.Now.ToLongTimeString()}");
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