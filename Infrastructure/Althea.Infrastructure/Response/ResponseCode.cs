using System.ComponentModel;

namespace Althea.Infrastructure.Response;

public static partial class ResponseCode
{
    [Description("success")]
    public static readonly int Success = 0;

    #region Error

    public static readonly int Error = 50000;

    #endregion

    #region Fail

    public static readonly int Fail = 40000;

    [Description("model invalid")]
    public static readonly int ModelInvalid = 40100;

    #region Not Found

    [Description("entity not found")]
    public static readonly int EntityNotFound = 40401;

    #endregion

    #endregion
}
