namespace BaseXml.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string input)
        {
            return input.Length > 0 ? input.Substring(0, 1).ToLower() + input.Substring(1) : input;
        }
    }
}
