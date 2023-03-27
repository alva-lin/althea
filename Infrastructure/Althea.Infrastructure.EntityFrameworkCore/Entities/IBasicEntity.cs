namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

public interface IBasicEntity
{
}

public interface IBasicEntity<TKey> : IBasicEntity where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
}
