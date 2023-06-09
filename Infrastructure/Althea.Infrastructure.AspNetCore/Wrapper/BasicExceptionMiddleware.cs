﻿using Althea.Infrastructure.Extensions;
using Althea.Infrastructure.Response;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Althea.Infrastructure.AspNetCore.Wrapper;

public class BasicExceptionMiddleware
{
    private readonly ILogger<BasicExceptionMiddleware> _logger;
    private readonly RequestDelegate                   _next;

    public BasicExceptionMiddleware(RequestDelegate next, ILogger<BasicExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cancellationToken = context.RequestAborted;
        try
        {
            await _next(context);
        }
        catch (BasicException e)
        {
            var result = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? ResponseResult.Error(e.ErrorInfos ?? null, e.Code, e.Message, e.StackTrace)
                : ResponseResult.Error<object>(null, e.Code, e.Message);

            await context.Response.WriteAsJsonAsync(result, cancellationToken);

            _logger.LogError(e, "{Code} {Message} {Info}",
                e.Code,
                e.Message,
                e.ErrorInfos.ToJson());
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("request canceled");
        }
        catch (Exception e)
        {
            var code = ResponseCode.Error;

            var result = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? ResponseResult.Error<object>(null, code, e.Message, e.StackTrace)
                : ResponseResult.Error<object>(null, code, e.Message);

            await context.Response.WriteAsJsonAsync(result, cancellationToken);

            _logger.LogError(e, "{Code} {Message} {Info}",
                code,
                e.Message,
                e.StackTrace);
        }
    }
}
