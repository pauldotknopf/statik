using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Markdig;
using Markdig.Syntax;
using YamlDotNet.Serialization;

namespace Statik.Markdown.Impl
{
    public class MarkdownParser : IMarkdownParser
    {
        public MarkdownParseResult<T> Parse<T>(string markdown) where T : class
        {
            if (string.IsNullOrEmpty(markdown))
            {
                return new MarkdownParseResult<T>(null, null);
            }

            markdown = Regex.Replace(markdown, @"(\r\n)|(\n\r)|(\n\r)|(\r)", Environment.NewLine);
            
            var builder = new MarkdownPipelineBuilder();
            builder.Extensions.Add(new Markdig.Extensions.Yaml.YamlFrontMatterExtension());
            var pipeline = builder.Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);
            var yamlBlocks = document.Descendants<Markdig.Extensions.Yaml.YamlFrontMatterBlock>()
                .ToList();
            
            if(yamlBlocks.Count == 0)
            {
                return new MarkdownParseResult<T>(null, markdown);
            }

            if(yamlBlocks.Count > 1)
            {
                throw new InvalidOperationException();
            }

            var yamlBlock = yamlBlocks.First();

            var yamlBlockIterator = yamlBlock.Lines.ToCharIterator();
            var yamlString = new StringBuilder();
            while (yamlBlockIterator.CurrentChar != '\0')
            {
                yamlString.Append(yamlBlockIterator.CurrentChar);
                yamlBlockIterator.NextChar();
            }

            var yamlDeserializer = new DeserializerBuilder().Build();
            var yamlObject = yamlDeserializer.Deserialize<T>(new StringReader(yamlString.ToString()));

            markdown = markdown.Substring(yamlBlock.Span.End + 1);
            if(markdown.StartsWith(Environment.NewLine))
                markdown = markdown.Substring(Environment.NewLine.Length);

            return new MarkdownParseResult<T>(yamlObject, markdown);
        }
    }
}