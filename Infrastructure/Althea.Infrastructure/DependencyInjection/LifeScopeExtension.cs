using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Althea.Infrastructure.DependencyInjection;

public static class LifeScopeExtension
{
    /// <summary>
    /// 将指定程序集中，带有 <see cref="LifeScopeAttribute"/> 特性的服务注册到容器中
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="assemblyPrefixArray">待遍历程序集的名称前缀数组，凡是符合这个名称前缀的程序集都会被遍历</param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IServiceCollection AddByLifeScope(this IServiceCollection services,
        params string[] assemblyPrefixArray)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assemblyPrefixArray.Any(prefix => assembly.FullName!.StartsWith(prefix)))
            .ToArray();

        return services.AddByLifeScope(assemblies);
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
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .ToArray();

        foreach (var type in types)
        {
            var lifeScopeAttrs = type.GetCustomAttributes<LifeScopeAttribute>(false);

            foreach (var attr in lifeScopeAttrs)
            {
                var implements = type.GetInterfaces().Where(iType => !iType.IsGenericType)
                    .Intersect(attr.Types)
                    .Union(new []{ type });
                foreach (var implement in implements)
                {
                    switch (attr.Scope)
                    {
                        case LifeScope.Singleton:
                            services.AddTransient(implement, type);
                            break;
                        case LifeScope.Scope:
                            services.AddScoped(implement, type);
                            break;
                        case LifeScope.Transient:
                            services.AddTransient(implement, type);
                            break;
                    }
                }
            }
        }

        return services;
    }
}
