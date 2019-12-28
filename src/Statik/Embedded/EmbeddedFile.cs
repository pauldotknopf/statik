namespace Statik.Embedded
{
    internal class EmbeddedFile
    {
        public EmbeddedFile(string path, string resourceName)
        {
            Path = path;
            ResourceName = resourceName;
        }

        public string Path { get; }

        public string ResourceName { get; }
    }
}