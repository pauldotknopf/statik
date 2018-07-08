using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Statik.Files
{
    public class RegisterOptions
    {
        public BuildStateDelegate State { get; set; }
        
        public Matcher Matcher { get; set; }
        
        public delegate object BuildStateDelegate(string requestPrefix, string requestFullPath, string filePath, IFileInfo file, IFileProvider fileProvider);
    }
}