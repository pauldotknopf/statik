using System;

namespace Statik.Markdown
{
    public interface IMarkdownRenderer
    {
        string Render(string markdown, Func<string, string> linkRewriter = null);
    }
}