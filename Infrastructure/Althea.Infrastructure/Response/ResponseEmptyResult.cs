namespace Althea.Infrastructure.Response;

public class ResponseEmptyResult : ResponseResult<VoidObject>
{
    #region Static Methods

    public static ResponseEmptyResult Success(string? message = null)
    {
        return new()
        {
            Data    = VoidObject.Instance,
            Code    = 0,
            Message = message
        };
    }

    public static ResponseEmptyResult Fail(int code, string? message = null, string? errorMessage = null)
    {
        return new()
        {
            Data         = VoidObject.Instance,
            Code         = code,
            Message      = message,
            ErrorMessage = errorMessage ?? code.ToDescription()
        };
    }

    public static ResponseEmptyResult Error(int code, string? message = null, string? errorMessage = null)
    {
        return new()
        {
            Data         = VoidObject.Instance,
            Code         = code,
            Message      = message,
            ErrorMessage = errorMessage ?? code.ToDescription()
        };
    }

    #endregion

    #region Implicit

    public static implicit operator ResponseEmptyResult(VoidObject data)
    {
        return Success();
    }

    public static implicit operator ResponseEmptyResult(string message)
    {
        return Success(message);
    }

    public static implicit operator ResponseEmptyResult((VoidObject Data, string Message) tuple)
    {
        return Success(tuple.Message);
    }

    #endregion
}
