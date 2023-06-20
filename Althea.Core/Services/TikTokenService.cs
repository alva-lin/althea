using Althea.Infrastructure;
using Althea.Infrastructure.DependencyInjection;
using OpenAI.ObjectModels;
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

    private readonly IDictionary<string, Models.Model> _modelDict;

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

        _modelDict = Enum.GetValues<Models.Model>().ToDictionary(Models.EnumToString, item => item);
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

    private GptEncoding GetGptEncoding(string model)
    {
        return TryGetGptEncoding(CL100K_BASE);
        if (!_modelDict.TryGetValue(model, out var modelEnum))
            throw new ArgumentOutOfRangeException(nameof(model), model, null);

        GptEncoding encoding;
        switch (modelEnum)
        {
            case Models.Model.Gpt_3_5_Turbo:
            case Models.Model.Gpt_3_5_Turbo_0613:
            case Models.Model.Gpt_3_5_Turbo_16k:
            case Models.Model.Gpt_3_5_Turbo_16k_0613:
            case Models.Model.Gpt_4:
            case Models.Model.Gpt_4_0314:
            case Models.Model.Gpt_4_32k:
            case Models.Model.Gpt_4_32k_0314:
            case Models.Model.TextEmbeddingAdaV2:
                encoding = TryGetGptEncoding(CL100K_BASE);
                break;
            case Models.Model.TextDavinciV3:
            case Models.Model.TextDavinciV2:
            case Models.Model.CodeDavinciV2:
            case Models.Model.CodeDavinciV1:
            case Models.Model.CodeCushmanV1:
                encoding = TryGetGptEncoding(P50K_BASE);
                break;
            case Models.Model.TextEditDavinciV1:
            case Models.Model.CodeEditDavinciV1:
                encoding = TryGetGptEncoding(P50K_EDIT);
                break;
            case Models.Model.TextDavinciV1:
            case Models.Model.TextCurieV1:
            case Models.Model.TextBabbageV1:
            case Models.Model.TextAdaV1:
            case Models.Model.Davinci:
            case Models.Model.Curie:
            case Models.Model.Babbage:
            case Models.Model.Ada:
            case Models.Model.TextSimilarityDavinciV1:
            case Models.Model.TextSimilarityCurieV1:
            case Models.Model.TextSimilarityBabbageV1:
            case Models.Model.TextSimilarityAdaV1:
            case Models.Model.TextSearchDavinciDocV1:
            case Models.Model.TextSearchCurieDocV1:
            case Models.Model.TextSearchBabbageDocV1:
            case Models.Model.TextSearchAdaDocV1:
            case Models.Model.CodeSearchBabbageCodeV1:
            case Models.Model.CodeSearchAdaCodeV1:
                encoding = TryGetGptEncoding(R50K_BASE);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(model), model, null);
        }

        return encoding;
    }

    /// <summary>
    ///     计算 Token 长度
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="model">模型</param>
    /// <returns>Token 长度</returns>
    /// <exception cref="ArgumentOutOfRangeException">传入的模型文本如果不能转换为 OpenAI 指定的模型，则会抛出错误</exception>
    public int CalculateTokenLength(string text, string? model = null)
    {
        return GetGptEncoding(model ?? Models.Gpt_3_5_Turbo_16k).Encode(text).Count;
    }
}
