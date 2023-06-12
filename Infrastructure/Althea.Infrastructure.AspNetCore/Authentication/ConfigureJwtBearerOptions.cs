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

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        options.Events ??= new JwtBearerEvents();

        var originalOnMessageReceived = options.Events.OnMessageReceived;
        options.Events.OnMessageReceived = async context =>
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (originalOnMessageReceived != null) await originalOnMessageReceived(context);

            if (string.IsNullOrEmpty(context.Token))
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub")) context.Token = accessToken;
            }
        };
    }
}