using Althea.Infrastructure.DependencyInjection;
using Minio;

namespace Althea.Data.Domains.StorageDomain;

public interface IStorageService
{
    Task<StorageObject> PutObjectAsync(string bucketName, Stream stream, string fileName, string contentType,
        Dictionary<string, string>? metaData = null, CancellationToken cancellationToken = default);

    Task DeleteObjectAsync(Guid id);

    Task<Stream> GetObjectAsync(Guid id, CancellationToken cancellationToken = default);

    Task<string> PresignedGetObjectAsync(Guid id, TimeSpan expires, Dictionary<string, string>? reqParams = null);

    Task<string> PresignedPutObjectAsync(Guid id, TimeSpan expires, Dictionary<string, string>? reqParams = null);
}

[LifeScope(LifeScope.Scope, typeof(IStorageService))]
public class StorageService : IStorageService
{
    private readonly IMinioClient _minioClient;

    private readonly AltheaDbContext _dbContext;

    private readonly ILogger<StorageService> _logger;

    public StorageService(IMinioClient minioClient, AltheaDbContext dbContext, ILogger<StorageService> logger)
    {
        _minioClient = minioClient;
        _dbContext = dbContext;
        _logger = logger;
    }

    private static string GeneratePath(string fileName)
    {
        return Guid.NewGuid() + "/" + fileName;
    }

    public async Task<StorageObject> PutObjectAsync(string bucketName, Stream stream, string fileName,
        string contentType, Dictionary<string, string>? metaData = null, CancellationToken cancellationToken = default)
    {
        var path = GeneratePath(fileName);
        var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(path)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType)
                .WithHeaders(metaData)
            ;

        try
        {
            await _minioClient.PutObjectAsync(args, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "上传文件: {FileName} 失败", fileName);
            throw new($"上传文件: {fileName} 失败");
        }

        var storageObject = new StorageObject
        {
            Bucket = bucketName,
            Path = path,
            FileName = fileName,
            Extension = Path.GetExtension(fileName),
            ContentType = contentType,
            Size = stream.Length
        };
        await _dbContext.Set<StorageObject>().AddAsync(storageObject, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return storageObject;
    }

    public async Task DeleteObjectAsync(Guid id)
    {
        var storageObject = await _dbContext.Set<StorageObject>().FindAsync(id);
        if (storageObject != null) _dbContext.Remove(storageObject);
    }

    public async Task<Stream> GetObjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var storageObject = await _dbContext.Set<StorageObject>().FindAsync(id);
        if (storageObject == null) throw new("文件不存在");

        var ms = new MemoryStream();
        var args = new GetObjectArgs()
                .WithBucket(storageObject.Bucket)
                .WithObject(storageObject.Path)
                .WithCallbackStream((stream, token) =>
                {
                    var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, cancellationToken);
                    return stream.CopyToAsync(ms, tokenSource.Token);
                })
            ;
        await _minioClient.GetObjectAsync(args, cancellationToken);
        return ms;
    }

    public async Task<string> PresignedGetObjectAsync(Guid id, TimeSpan expires,
        Dictionary<string, string>? reqParams = null)
    {
        var storageObject = await _dbContext.Set<StorageObject>().FindAsync(id);
        if (storageObject == null) throw new("文件不存在");

        var args = new PresignedGetObjectArgs()
                .WithBucket(storageObject.Bucket)
                .WithObject(storageObject.Path)
                .WithExpiry(expires.Seconds)
                .WithHeaders(reqParams)
            ;
        var url = await _minioClient.PresignedGetObjectAsync(args);
        return url;
    }

    public async Task<string> PresignedPutObjectAsync(Guid id, TimeSpan expires,
        Dictionary<string, string>? reqParams = null)
    {
        var storageObject = await _dbContext.Set<StorageObject>().FindAsync(id);
        if (storageObject == null) throw new("文件不存在");

        var args = new PresignedPutObjectArgs()
                .WithBucket(storageObject.Bucket)
                .WithObject(storageObject.Path)
                .WithExpiry(expires.Seconds)
                .WithHeaders(reqParams)
            ;
        var url = await _minioClient.PresignedPutObjectAsync(args);
        return url;
    }
}
