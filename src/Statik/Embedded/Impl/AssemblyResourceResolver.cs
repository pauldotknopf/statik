using System.IO;
using System.Reflection;

namespace Statik.Embedded.Impl
{
    public class AssemblyResourceResolver : IAssemblyResourceResolver
    {
        readonly Assembly _assembly;

        public AssemblyResourceResolver(Assembly assembly)
        {
            _assembly = assembly;
        }

        public string[] GetManifestResourceNames()
        {
            return _assembly.GetManifestResourceNames();
        }

        public Stream GetManifestResourceStream(string name)
        {
            return _assembly.GetManifestResourceStream(name);
        }
    }
}