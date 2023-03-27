using Alva.Toolkit.AspNetCore.Wrapper;

using Microsoft.AspNetCore.Builder;

namespace Alva.Toolkit.AspNetCore.Extensions;

public static class MiddlewaveExtension
{
    public static IApplicationBuilder UseBasicException(this IApplicationBuilder host)
    {
        host.UseMiddleware<BasicExceptionMiddleware>();

        return host;
    }
}
