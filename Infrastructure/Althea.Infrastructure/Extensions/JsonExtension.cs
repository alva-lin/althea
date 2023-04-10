using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Althea.Infrastructure.JsonConverters;

namespace Althea.Infrastructure.Extensions;

public static class JsonExtension
{
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions;

    static JsonExtension()
    {
        DefaultJsonSerializerOptions = new();
        SetDefaultJsonSerializerOptions(DefaultJsonSerializerOptions);
    }

    public static void SetDefaultJsonSerializerOptions(JsonSerializerOptions options)
    {
        options.PropertyNameCaseInsensitive = true;
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.WriteIndented = true;
        options.MaxDepth = 32;

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
        options.Converters.Add(new DateOnlyJsonConverter("yyyy-MM-dd"));
        options.Converters.Add(new TimeOnlyJsonConverter("HH:mm:ss"));
    }

    public static string ToJson(this object? obj, int maxDeep = 32)
    {
        var jsonSerializerOptions = new JsonSerializerOptions(DefaultJsonSerializerOptions)
        {
            MaxDepth = maxDeep
        };
        return JsonSerializer.Serialize(obj, jsonSerializerOptions);
    }
}
