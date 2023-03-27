using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Althea.Infrastructure.EntityFrameworkCore.Converters;

public class DateTimeToUtcConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeToUtcConverter()
        : base(to => TimeZoneInfo.ConvertTimeToUtc(to),
            from => from)
    {

    }
}
