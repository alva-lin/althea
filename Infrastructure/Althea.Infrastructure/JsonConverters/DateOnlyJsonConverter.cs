using System.Text.Json;
using System.Text.Json.Serialization;

namespace Althea.Infrastructure.JsonConverters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private readonly string _format;

    public DateOnlyJsonConverter(string format)
    {
        _format = format;
    }

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format));
    }
}