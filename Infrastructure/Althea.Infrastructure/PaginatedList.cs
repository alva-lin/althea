namespace Althea.Infrastructure;

/// <summary>
/// 分页数组
/// </summary>
/// <typeparam name="T">数组项的类型</typeparam>
public record PaginatedList<T>
{
    /// <summary>
    /// 当前页的数据
    /// </summary>
    public T[] Data { get; protected set; } = Array.Empty<T>();

    /// <summary>
    /// 总数量
    /// </summary>
    public int Count { get; protected set; }

    /// <summary>
    /// long 类型的总数量
    /// </summary>
    public long LongCount { get; protected set; }

    protected PaginatedList() {}

    public PaginatedList(T[] data, long longCount)
    {
        Data      = data;
        LongCount = longCount;
        Count     = (int) longCount;
    }

    /// <summary>
    /// 从 <see cref="IQueryable{T}"/> 中异步创建一个分页数组
    /// </summary>
    /// <param name="source">需要获取分页数组的 <see cref="IQueryable{T}"/></param>
    /// <param name="skip">跳过的数量</param>
    /// <param name="take">获取的数量</param>
    /// <typeparam name="T">数据源的类型</typeparam>
    /// <returns></returns>
    public static PaginatedList<T> Create(IQueryable<T> source, int skip, int take)
    {
        return new()
        {
            Data      = source.Skip(skip).Take(take).ToArray(),
            Count     = source.Count(),
            LongCount = source.LongCount()
        };
    }
}
