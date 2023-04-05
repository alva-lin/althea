using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Althea.Infrastructure.Extensions;

public static class JsonExtension
{
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler     = ReferenceHandler.IgnoreCycles,
        Encoder              = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented        = true
    };

    public static string ToJson(this object? obj)
    {
        return JsonSerializer.Serialize(obj, DefaultJsonSerializerOptions);
    }
}
