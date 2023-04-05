namespace Althea.Infrastructure.DependencyInjection;

/// <summary>
///     用于标识服务的生命周期，以及被注册的类型
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
public class LifeScopeAttribute : Attribute
{
    public LifeScopeAttribute(LifeScope scope, params Type[] types)
    {
        Scope = scope;
        Types = types;
    }

    /// <summary>
    ///     生命周期
    /// </summary>
    public LifeScope Scope { get; set; }

    /// <summary>
    ///     注册类型的数组
    /// </summary>
    public Type[] Types { get; set; }
}
