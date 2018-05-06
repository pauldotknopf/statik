using System.Threading.Tasks;

namespace Statik.Hosting
{
    public interface IHostExporter
    {
        Task Export(IHost host, string destinationDirectory);
    }
}