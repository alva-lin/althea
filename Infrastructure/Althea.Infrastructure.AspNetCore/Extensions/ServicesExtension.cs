using System.Reflection;

using Alva.Toolkit.AspNetCore.Authentication;
using Alva.Toolkit.AspNetCore.Cors;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Alva.Toolkit.AspNetCore.Extensions;

public static class ServicesExtension
{
    public static void IncludeAllXmlComments(this SwaggerGenOptions options)
    {
        var basePath = Directory.GetParent(Environment.CurrentDirectory);
        if (basePath is { Exists: true })
        {
            var currentAssembly = Assembly.GetCallingAssembly();
            var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union(new[] { currentAssembly.GetName() })
                .Select(a => Path.Combine(basePath.FullName, a.Name!, $"{a.Name}.xml"))
                .Where(File.Exists)
                .ToArray();
            Array.ForEach(xmlDocs, s => options.IncludeXmlComments(s));
        }
    }

    public static IServiceCollection AddJwtBearer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.Configure<JwtOption>(configuration.GetSection(nameof(JwtOption)));

        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        return services;
    }

    public static IServiceCollection AddCorsSetting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorsOption>(configuration.GetSection(nameof(CorsOption)));

        services.AddCors(options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var corsOption      = serviceProvider!.GetRequiredService<IOptions<CorsOption>>().Value;

            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(corsOption.AllowOrigins).WithHeaders(corsOption.AllowHeaders);
            });
        });

        return services;
    }
}
