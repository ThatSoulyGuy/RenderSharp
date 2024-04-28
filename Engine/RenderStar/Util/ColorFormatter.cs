using System.Text.RegularExpressions;

namespace RenderStar.Util
{
    public static partial class ColorFormatter
    {
        private static Dictionary<char, string> RawToANSI { get; } = new()
        {
            {'0', "\u001b[30m"},
            {'1', "\u001b[34m"},
            {'2', "\u001b[32m"},
            {'3', "\u001b[36m"},
            {'4', "\u001b[31m"},
            {'5', "\u001b[35m"},
            {'6', "\u001b[33m"},
            {'7', "\u001b[37m"},
            {'8', "\u001b[90m"},
            {'9', "\u001b[94m"},
            {'a', "\u001b[92m"},
            {'b', "\u001b[96m"},
            {'c', "\u001b[91m"},
            {'d', "\u001b[95m"},
            {'e', "\u001b[93m"},
            {'f', "\u001b[97m"},
            {'r', "\u001b[0m"},
            {'n', "\u001b[4m"},
            {'l', "\u001b[1m"},
            {'o', "\u001b[3m"}
        };

        public static string Format(string input)
        {
            return FormatRegex().Replace(input, match =>
            {
                char key = match.Groups[1].Value[0];

                if (RawToANSI.TryGetValue(key, out string? value))
                    return value;

                return match.Groups[0].Value;
            });
        }

        public static string Deformat(string input)
        {
            string removeAnsi = RemovalRegex().Replace(input, string.Empty);

            return RemovalRelplaceRegex().Replace(removeAnsi, string.Empty);
        }

        [GeneratedRegex(@"\e\[[0-9;]*m")]
        private static partial Regex RemovalRegex();

        [GeneratedRegex(@"&[0-9a-fk-lnor]")]
        private static partial Regex RemovalRelplaceRegex();

        [GeneratedRegex(@"&([0-9a-fk-lnor])")]
        private static partial Regex FormatRegex();
    }
}