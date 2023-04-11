using Microsoft.AspNetCore.Http;

namespace Althea.Infrastructure.AspNetCore;

public class JwtAuthInfoProvider : IAuthInfoProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    public JwtAuthInfoProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string CurrentUser => _contextAccessor.HttpContext?.User.Identity?.Name ?? "Unknown";
}