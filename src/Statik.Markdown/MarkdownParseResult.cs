namespace Statik.Markdown
{
    public class MarkdownParseResult<T>
    {
        public MarkdownParseResult(T yaml, string markdown)
        {
            Yaml = yaml;
            Markdown = markdown;
        }
        
        public T Yaml { get; }
        
        public string Markdown { get; }
    }
}