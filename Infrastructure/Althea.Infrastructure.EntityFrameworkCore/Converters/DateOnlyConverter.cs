using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Althea.Infrastructure.EntityFrameworkCore.Converters;

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(
            dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            time => DateOnly.FromDateTime(time))
    {
    }
}
