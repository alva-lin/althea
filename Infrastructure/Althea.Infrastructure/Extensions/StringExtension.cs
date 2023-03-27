using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web;

namespace Althea.Infrastructure.Extensions;

public static class StringExtension
{
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? str)
    {
        return !string.IsNullOrEmpty(str);
    }

    public static string ToCamelCase(this string str)
    {
        if (!str.IsNullOrWhiteSpace() && str.Length > 1)
        {
            return str[0].ToString().ToLowerInvariant() + str.Substring(1);
        }
        return str.ToLowerInvariant();
    }

    public static string UrlEncode(this string str, Encoding? e = null)
    {
        if (e == null)
        {
            return HttpUtility.UrlEncode(str);
        }
        return HttpUtility.UrlEncode(str, e);
    }

    public static string UrlDecode(this string str, Encoding? e = null)
    {
        if (e == null)
        {
            return HttpUtility.UrlDecode(str);
        }
        return HttpUtility.UrlDecode(str, e);
    }

    public static string TrimStart(this string str, string prefix)
    {
        if (str.StartsWith(prefix))
        {
            return str.Substring(prefix.Length);
        }
        return str;
    }

    public static string TrimEnd(this string str, string suffix)
    {
        if (str.EndsWith(suffix))
        {
            return str.Substring(0, str.Length - suffix.Length);
        }
        return str;
    }

    public static string Trim(this string str, string pattern)
    {
        return str.TrimStart(pattern).TrimEnd(pattern);
    }
}
