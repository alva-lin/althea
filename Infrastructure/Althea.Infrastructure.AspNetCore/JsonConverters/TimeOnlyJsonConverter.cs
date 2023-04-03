using System.Text.Json;
using System.Text.Json.Serialization;

namespace Althea.Infrastructure.AspNetCore.JsonConverters;

public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    private readonly string _format;

    public TimeOnlyJsonConverter(string format)
    {
        _format = format;
    }

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeOnly.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format));
    }
}
