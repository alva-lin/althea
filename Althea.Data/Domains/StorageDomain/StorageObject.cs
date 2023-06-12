namespace Althea.Data.Domains.StorageDomain;

public class StorageObject : BasicEntity<Guid>
{
    /// <summary>
    ///     存储桶
    /// </summary>
    public string Bucket { get; set; } = null!;

    /// <summary>
    ///     路径
    /// </summary>
    public string Path { get; set; } = null!;

    /// <summary>
    ///     文件名
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    ///     扩展名
    /// </summary>
    public string Extension { get; set; } = null!;

    /// <summary>
    ///     完整文件名
    /// </summary>
    public string FullName => $"{FileName}.{Extension}";

    /// <summary>
    ///     文件类型
    /// </summary>
    public string ContentType { get; set; } = null!;

    /// <summary>
    ///     文件大小
    /// </summary>
    public long Size { get; set; }
}
