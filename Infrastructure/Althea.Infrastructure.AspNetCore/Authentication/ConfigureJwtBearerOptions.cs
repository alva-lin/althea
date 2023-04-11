using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Althea.Infrastructure.AspNetCore.Authentication;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOption _jwtOption;

    private readonly bool _isDevelopment;

    public ConfigureJwtBearerOptions(IOptions<JwtOption> jwtOption, IHostEnvironment environment)
    {
        _jwtOption = jwtOption.Value;
        _isDevelopment = environment.IsDevelopment();
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }

    public void Configure(JwtBearerOptions options)
    {
        options.Audience = _jwtOption.Audience;
        options.MetadataAddress = _jwtOption.MetaDataAddress;
        options.RequireHttpsMetadata = !_isDevelopment;
        // options.TokenValidationParameters = new()
        // {
        //     ValidateIssuer = true,
        //     ValidIssuer = _jwtOption.Authority,
        //
        //     ValidateAudience = true,
        //     ValidAudience = _jwtOption.Audience,
        //
        //     ValidateLifetime = true,
        //
        //     ValidateIssuerSigningKey = false,
        // };
    }
}