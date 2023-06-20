using Althea.Infrastructure;
using Althea.Infrastructure.DependencyInjection;
using SharpToken;

// ReSharper disable InconsistentNaming

namespace Althea.Core.Services;

/// <summary>
///     Token 计算服务
/// </summary>
[LifeScope(LifeScope.Singleton)]
public class TikTokenService : IService
{
    private const string CL100K_BASE = "cl100k_base";

    private const string P50K_BASE = "p50k_base";

    private const string P50K_EDIT = "p50k_edit";

    private const string R50K_BASE = "r50k_base";

    private readonly IDictionary<string, GptEncoding> _encodings;

    public TikTokenService()
    {
        var arr = new[]
        {
            R50K_BASE,
            P50K_BASE,
            P50K_EDIT,
            CL100K_BASE
        };
        _encodings = arr.ToDictionary(item => item, GptEncoding.GetEncoding);
    }

    private GptEncoding TryGetGptEncoding(string name)
    {
        if (!_encodings.TryGetValue(name, out var encoding))
        {
            encoding = GptEncoding.GetEncoding(name);
            _encodings[name] = encoding;
        }

        return encoding;
    }

    private GptEncoding GetGptEncoding()
    {
        return TryGetGptEncoding(CL100K_BASE);
    }

    /// <summary>
    ///     计算 Token 长度
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>Token 长度</returns>
    /// <exception cref="ArgumentOutOfRangeException">传入的模型文本如果不能转换为 OpenAI 指定的模型，则会抛出错误</exception>
    public int CalculateTokenLength(string text)
    {
        return GetGptEncoding().Encode(text).Count;
    }
}
