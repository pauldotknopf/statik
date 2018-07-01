using System.Text.RegularExpressions;

namespace Statik
{
    public class StatikHelpers
    {
        public static string ConvertStringToSlug(string value)
        {
            return Regex.Replace(value, @"[^A-Za-z0-9_\.~]+", "-");
        }
    }
}