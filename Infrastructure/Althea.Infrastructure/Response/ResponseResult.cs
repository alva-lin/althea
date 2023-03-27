// ReSharper disable MemberCanBeProtected.Global

namespace Althea.Infrastructure.Response;

/// <summary>
///     接口返回值包装类
/// </summary>
/// <typeparam name="TEntity"></typeparam>
// ReSharper disable once ConvertToStaticClass
public class ResponseResult<TEntity>
{
    /// <summary>
    ///     返回值代码
    /// </summary>
    public int Code { get; init; }

    /// <summary>
    ///     返回值
    /// </summary>
    public TEntity? Data { get; init; }

    /// <summary>
    ///     消息
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    ///     错误消息
    /// </summary>
    public string? ErrorMessage { get; init; }

    #region Implicit

    public static implicit operator ResponseResult<TEntity>(TEntity? data)
    {
        return ResponseResult.Success(data);
    }

    public static implicit operator ResponseResult<TEntity>((TEntity? Data, string? Message) tuple)
    {
        return ResponseResult.Success(tuple.Data, tuple.Message);
    }

    #endregion
}

public static class ResponseResult
{
    public static ResponseResult<T> Success<T>(T? data, string? message = null)
    {
        return new()
        {
            Data    = data,
            Code    = 0,
            Message = message
        };
    }

    public static ResponseResult<T> Fail<T>(T? data, int code, string? message = null, string? errorMessage = null)
    {
        return new()
        {
            Data         = data,
            Code         = code,
            Message      = message,
            ErrorMessage = errorMessage ?? code.ToDescription()
        };
    }

    public static ResponseResult<T> Error<T>(T? data, int code, string? message = null, string? errorMessage = null)
    {
        return new()
        {
            Data         = data,
            Code         = code,
            Message      = message,
            ErrorMessage = errorMessage ?? code.ToDescription()
        };
    }
}
