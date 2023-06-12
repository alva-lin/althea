namespace Althea;

/// <summary>
///     系统设置
/// </summary>
public sealed class SystemOption
{
}

/// <summary>
///     MinIO 设置
/// </summary>
public sealed class MinIOOption
{
    /// <summary>
    ///     端点
    /// </summary>
    public string EndPoint { get; set; } = null!;

    /// <summary>
    ///     访问密钥
    /// </summary>
    public string AccessKey { get; set; } = null!;

    /// <summary>
    ///     密钥
    /// </summary>
    public string SecretKey { get; set; } = null!;

    /// <summary>
    ///     是否使用安全连接
    /// </summary>
    public bool UseSecure { get; set; } = false;
}
