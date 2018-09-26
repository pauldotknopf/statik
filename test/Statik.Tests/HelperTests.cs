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
            var result = StatikHelpers.ResolvePathPart(input, relative);
            Assert.Equal(expected, result);
        }
    }
}