namespace Alva.Toolkit.AspNetCore.Cors;

/// <summary>
///     Cors 配置
/// </summary>
public class CorsOption
{
    public string[] AllowOrigins { get; set; } = Array.Empty<string>();

    public string[] AllowHeaders { get; set; } = Array.Empty<string>();
}
