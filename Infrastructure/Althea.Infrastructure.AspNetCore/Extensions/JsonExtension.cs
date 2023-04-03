using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

using Althea.Infrastructure.AspNetCore.JsonConverters;

using Microsoft.AspNetCore.Mvc;

namespace Althea.Infrastructure.AspNetCore.Extensions;

public static class JsonExtension
{
    public static JsonOptions ConfigureDefaultOptions(this JsonOptions options)
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler     = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Encoder              = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.JsonSerializerOptions.WriteIndented        = true;

        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter("yyyy-MM-dd"));
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter("HH:mm:ss"));

        return options;
    }
}
