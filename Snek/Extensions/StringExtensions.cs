using System.Text.RegularExpressions;

namespace Snek.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex("[a-z][A-Z]")]
    private static partial Regex azAZRegex();

    public static string ToSentenceCase(this string str)
        => azAZRegex().Replace(str, m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
}