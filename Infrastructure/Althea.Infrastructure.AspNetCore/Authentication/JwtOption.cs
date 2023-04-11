namespace Althea.Infrastructure.AspNetCore.Authentication;

/// <summary>
///     Jwt 配置
/// </summary>
public class JwtOption
{
    /// <summary>
    ///     接收者
    /// </summary>
    public string Audience { get; set; } = null!;

    /// <summary>
    ///     OIDC 地址
    /// </summary>
    public string MetaDataAddress { get; set; } = null!;
}
