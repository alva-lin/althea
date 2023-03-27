using System.ComponentModel;
using System.Reflection;

using Althea.Infrastructure.Extensions;

namespace Althea.Infrastructure.Response;

public static partial class ResponseCode
{
    private static readonly Dictionary<int, string> Cache;

    static ResponseCode()
    {
        var fields = typeof(ResponseCode).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(info => info.FieldType == typeof(int));

        Cache = new();

        foreach (var field in fields)
        {
            var description = field.Name.ToCamelCase();
            var attr = field.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
            if (attr != null)
            {
                description = attr.Description;
            }

            var key = (int)(field.GetValue(null) ?? -1);
            if (!Cache.TryGetValue(key, out _))
            {
                Cache.Add(key, description);
            }
        }
    }

    public static string ToDescription(this int code)
    {
        if (Cache.TryGetValue(code, out var value))
        {
            return value;
        }

        return "unknown code";
    }
}
