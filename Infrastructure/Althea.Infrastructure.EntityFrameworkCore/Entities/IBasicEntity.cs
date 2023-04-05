namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

public interface IAuditable
{
    [NotMapped]
    public IAudit Audit { get; set; }
}

public interface IAuditable<T> : IAuditable where T : IAudit
{
    public new T Audit { get; set; }

    [NotMapped]
    IAudit IAuditable.Audit
    {
        get => Audit;
        set => Audit = (T)value;
    }
}

public interface IBasicEntity
{
}

public interface IBasicEntity<TKey> : IBasicEntity where TKey : IEquatable<TKey>
{
    /// <summary>
    ///     Id
    /// </summary>
    public TKey Id { get; set; }
}

public interface IBasicEntity<TKey, TAudit> : IBasicEntity<TKey>, IAuditable<TAudit>
    where TKey : IEquatable<TKey>
    where TAudit : IAudit
{
}
