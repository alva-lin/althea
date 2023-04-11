using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Althea.Infrastructure.AspNetCore.Wrapper;

public class JwtParsingMiddleware
{
    private readonly RequestDelegate _next;

    public JwtParsingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authorization))
        {
            var token = authorization.ToString()["Bearer ".Length..].Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();
            var claimsIdentity = new ClaimsIdentity(claims, "jwt");
            claimsIdentity.AddClaim(new(ClaimTypes.Name, jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value!));
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            context.User = claimsPrincipal;
        }

        await _next(context);
    }
}