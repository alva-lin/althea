namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

#pragma warning disable CS8618
public interface IAuditable<T> where T : IAudit
{
    public T Audit { get; set; }
}
