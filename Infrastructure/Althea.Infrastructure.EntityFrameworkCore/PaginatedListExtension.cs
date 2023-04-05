namespace Althea.Infrastructure.EntityFrameworkCore;

public static class PaginatedListExtension
{
    /// <summary>
    ///     从 <see cref="IQueryable{T}" /> 中异步创建一个分页数组
    /// </summary>
    /// <param name="source">需要获取分页数组的 <see cref="IQueryable{T}" /></param>
    /// <param name="skip">跳过的数量</param>
    /// <param name="take">获取的数量</param>
    /// <param name="token">等待任务完成时用于观察的 <see cref="CancellationToken" /></param>
    /// <typeparam name="T">数据源的类型</typeparam>
    /// <returns></returns>
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int skip,
        int take, CancellationToken token = default)
    {
        var count = await source.LongCountAsync(token);

        var items = await source.Skip(skip).Take(take).ToArrayAsync(token);

        return new(items, count);
    }
}
