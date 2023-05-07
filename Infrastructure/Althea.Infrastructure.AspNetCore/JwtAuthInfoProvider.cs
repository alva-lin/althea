using Microsoft.AspNetCore.Http;

namespace Althea.Infrastructure.AspNetCore;

public class JwtAuthInfoProvider : IAuthInfoProvider
{
    private const string UnknownUser = "Unknown";

    public JwtAuthInfoProvider(IHttpContextAccessor contextAccessor)
    {
        CurrentUser = contextAccessor.HttpContext?.User.Identity?.Name ?? UnknownUser;
    }

    public void SetCurrentUser(string user)
    {
        CurrentUser = user;
    }

    public string CurrentUser { get; private set; }
}
