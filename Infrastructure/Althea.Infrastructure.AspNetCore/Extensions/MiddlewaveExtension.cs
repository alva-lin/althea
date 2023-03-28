using Althea.Infrastructure.AspNetCore.Wrapper;

using Microsoft.AspNetCore.Builder;

namespace Althea.Infrastructure.AspNetCore.Extensions;

public static class MiddlewaveExtension
{
    public static IApplicationBuilder UseBasicException(this IApplicationBuilder host)
    {
        host.UseMiddleware<BasicExceptionMiddleware>();

        return host;
    }
}
