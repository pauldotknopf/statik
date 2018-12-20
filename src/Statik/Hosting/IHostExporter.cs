using System.Threading.Tasks;

namespace Statik.Hosting
{
    public interface IHostExporter
    {
        Task Export(IHost host, string destinationDirectory);

        Task ExportParallel(IHost host, string destinationDirectory, int? maxThreads = null);
    }
}