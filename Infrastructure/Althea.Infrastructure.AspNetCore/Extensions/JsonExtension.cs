using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        return options;
    }
}
