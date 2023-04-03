using System.Diagnostics;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Althea.Infrastructure.DependencyInjection;

public static class LifeScopeExtension
{
    /// <summary>
    /// 将指定程序集中，带有 <see cref="LifeScopeAttribute"/> 特性的服务注册到容器中
    /// </summary>
    /// <remarks>启动项目可能会没有引用一些二级项目/程序集，所以会尝试遍历第二层的程序集</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="assemblyPrefixArray">待遍历程序集的名称前缀数组，凡是符合这个名称前缀的程序集都会被遍历</param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IServiceCollection AddByLifeScope(this IServiceCollection services,
        params string[] assemblyPrefixArray)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assemblyPrefixArray.Any(prefix => assembly.FullName!.StartsWith(prefix)))
            .ToArray();
        var twice = assemblies.SelectMany(assembly =>
                assembly.GetReferencedAssemblies().Where(name =>
                    assemblyPrefixArray.Any(prefix => name.FullName!.StartsWith(prefix))))
            .Distinct()
            .Select(Assembly.Load);

        return services.AddByLifeScope(assemblies.Union(twice).Distinct().ToArray());
    }

    /// <summary>
    /// 将指定程序集中，带有 <see cref="LifeScopeAttribute"/> 特性的服务注册到容器中
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="assemblies">待遍历的程序集数组</param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IServiceCollection AddByLifeScope(this IServiceCollection services, params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false });

        foreach (var type in types)
        {
            var lifeScopeAttrs = type.GetCustomAttributes<LifeScopeAttribute>(false);

            foreach (var attr in lifeScopeAttrs)
            {
                var interfaces = attr.Types.Union(new[] { type });
                foreach (var iType in interfaces)
                {
                    switch (attr.Scope)
                    {
                        case LifeScope.Singleton:
                            services.AddSingleton(iType, type);
                            break;
                        case LifeScope.Scope:
                            services.AddScoped(iType, type);
                            break;
                        case LifeScope.Transient:
                            services.AddTransient(iType, type);
                            break;
                    }
                }
            }
        }

        return services;
    }
}
