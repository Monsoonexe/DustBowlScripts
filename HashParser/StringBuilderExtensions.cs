using System.Text;

namespace HashParser
{
    internal static class StringBuilderExtensions
    {
        public static string ToStringAndClear(this StringBuilder sb)
        {
            string result = sb.ToString();
            sb.Clear();
            return result;
        }
    }
}
