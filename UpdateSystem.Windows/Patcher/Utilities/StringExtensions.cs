using System.Text.RegularExpressions;

namespace CodeElements.UpdateSystem.Windows.Patcher.Utilities
{
    internal static class StringExtensions
    {
        public static string WhiteSpacePascalCase(this string value)
        {
            return Regex.Replace(value, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }
    }
}