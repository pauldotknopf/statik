using System;
using System.IO;

namespace Statik.Embedded
{
    public interface IAssemblyResourceResolver
    {
        string[] GetManifestResourceNames();

        Stream GetManifestResourceStream(string name);
    }
}