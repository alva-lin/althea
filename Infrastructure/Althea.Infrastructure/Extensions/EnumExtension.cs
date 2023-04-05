using System.ComponentModel;
using System.Reflection;

namespace Althea.Infrastructure.Extensions;

public static class EnumExtension
{
    /// <summary>
    ///     获取枚举的描述文本，从 <see cref="DescriptionAttribute" /> 获取，如果没有，则直接返回枚举项的名称
    /// </summary>
    /// <param name="enum"></param>
    /// <returns></returns>
    public static string ToDescription(this Enum @enum)
    {
        var description = @enum.ToString("G");
        var field       = @enum.GetType().GetField(@enum.ToString());
        var attr        = field?.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
        if (attr is not null)
        {
            description = attr.Description;
        }
        return description;
    }

    /// <summary>
    ///     获取枚举的字典列表
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <returns>枚举字典项数组</returns>
    public static EnumKeyPair[] GetKeyPairs<T>() where T : Enum
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);

        return fields.Select(field =>
        {
            var attr = field.GetCustomAttribute<DescriptionAttribute>(false);
            return new EnumKeyPair(attr?.Description ?? field.Name, (int)field.GetValue(null)!);
        }).ToArray();
    }

    /// <summary>
    ///     枚举的字典项
    /// </summary>
    /// <param name="Description">描述文本，从枚举项的 <see cref="DescriptionAttribute" /> 获取，如果没有，则直接返回枚举项的名称</param>
    /// <param name="Value">值</param>
    public sealed record EnumKeyPair(string Description, int Value);
}
