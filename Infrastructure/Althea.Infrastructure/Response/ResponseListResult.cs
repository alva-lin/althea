namespace Althea.Infrastructure.Response;

public class ResponseListResult<TEntity> : ResponseResult<List<TEntity>>
{
    public int? TotalCount { get; set; }

    #region Implicit

    public static implicit operator ResponseListResult<TEntity>(List<TEntity> data)
    {
        return ResponseListResult.Success(data);
    }

    public static implicit operator ResponseListResult<TEntity>((List<TEntity> Data, int TotalCount) tuple)
    {
        return ResponseListResult.Success(tuple.Data, tuple.TotalCount);
    }

    public static implicit operator ResponseListResult<TEntity>((List<TEntity> Data, string Message) tuple)
    {
        return ResponseListResult.Success(tuple.Data, message: tuple.Message);
    }

    public static implicit operator ResponseListResult<TEntity>((List<TEntity> Data, int TotalCount, string Message) tuple)
    {
        return ResponseListResult.Success(tuple.Data, tuple.TotalCount, tuple.Message);
    }

    #endregion
}

public static class ResponseListResult
{
    public static ResponseListResult<TEntity> Success<TEntity>(List<TEntity> data, int? totalCount = null, string? message = null)
    {
        return new()
        {
            Data       = data,
            TotalCount = totalCount,
            Code       = 0,
            Message    = message
        };
    }
}
