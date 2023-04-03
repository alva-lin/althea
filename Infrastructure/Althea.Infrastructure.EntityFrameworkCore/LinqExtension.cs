using System.Linq.Expressions;

namespace Althea.Infrastructure.EntityFrameworkCore;

public static class LinqExtension
{
    public static IQueryable<TEntity> WhereIf<TEntity>(this IQueryable<TEntity> source, bool condition,
        Expression<Func<TEntity, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Include if condition is true, otherwise return source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="condition"></param>
    /// <param name="predicate"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> IncludeIf<TEntity>(this IQueryable<TEntity> source, bool condition,
        Expression<Func<TEntity, object>> predicate)
        where TEntity : class
    {
        return condition ? source.Include(predicate) : source;
    }
}
