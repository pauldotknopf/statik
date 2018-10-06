namespace Statik.Markdown
{
    public interface IMarkdownParser
    {
        MarkdownParseResult<T> Parse<T>(string markdown) where T : class;
    }
}