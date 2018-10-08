using System;
using System.IO;
using Markdig;
using Markdig.Renderers;

namespace Statik.Markdown.Impl
{
    public class MarkdownRenderer : IMarkdownRenderer
    {
        private readonly MarkdownPipeline _pipeline;
        
        public MarkdownRenderer(MarkdownPipeline pipeline = null)
        {
            if (pipeline == null)
            {
                pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .Build();
            }

            _pipeline = pipeline;
        }
        
        public string Render(string markdown, Func<string, string> linkRewriter = null)
        {
            var writer = new StringWriter();
            var renderer = new HtmlRenderer(writer);
            renderer.LinkRewriter = linkRewriter;

            _pipeline.Setup(renderer);
            
            var document = Markdig.Markdown.Parse(markdown, _pipeline);
            renderer.Render(document);
            writer.Flush();

            return writer.ToString();
        }
    }
}