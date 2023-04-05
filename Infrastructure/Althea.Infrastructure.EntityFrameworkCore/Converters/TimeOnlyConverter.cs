using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Althea.Infrastructure.EntityFrameworkCore.Converters;

public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
{
    public TimeOnlyConverter()
        : base(
            timeOnly => timeOnly.ToTimeSpan(),
            timeSpan => TimeOnly.FromTimeSpan(timeSpan))
    {
    }
}
