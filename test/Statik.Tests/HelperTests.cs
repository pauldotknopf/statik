using FluentAssertions;
using Xunit;

namespace Statik.Tests
{
    public class HelperTests
    {
        [Theory]
        [InlineData("/test", "test2", "/test/test2")]
        [InlineData("/test", "/test2", "/test2")]
        [InlineData("/test", "../test2", "/test2")]
        [InlineData("/test", "../test2/../test3", "/test3")]
        [InlineData("/test?query=ignored", "test2", "/test/test2")]
        [InlineData("/test?query=ignored", "test2?query=preserved", "/test/test2?query=preserved")]
        [InlineData("/test?#ignored", "test2", "/test/test2")]
        [InlineData("/test?#ignored", "test2#preserved", "/test/test2#preserved")]
        public void Can_resolve_relative_path(string input, string relative, string expected)
        {
            StatikHelpers.ResolvePathPart(input, relative).Should().Be(expected);
        }

        [Theory]
        [InlineData("test", "test")]
        [InlineData("--test--", "test")]
        [InlineData(".test.", "test")]
        [InlineData("test----test", "test-test")]
        public void Can_convert_string_to_slug(string input, string expected)
        {
            StatikHelpers.ConvertStringToSlug(input).Should().Be(expected);
        }
    }
}