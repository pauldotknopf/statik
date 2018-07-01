using System;
using System.Threading.Tasks;

namespace Statik.Examples.Pages
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var webBuilder = Statik.GetWebBuilder();
            
            // TODO:
            
            using (var host = webBuilder.BuildWebHost())
            {
                host.Listen();
                Console.WriteLine($"Listening on port {StatikDefaults.DefaultPort}...");
                Console.WriteLine($"Try visiting http://localhost:{StatikDefaults.DefaultPort}/hello");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                
            }
        }
    }
}
