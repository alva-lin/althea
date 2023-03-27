using Althea.Infrastructure;
using Althea.Infrastructure.Response;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Alva.Toolkit.AspNetCore.Wrapper;

public class ModelValidFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(pair => pair.Key, pair => pair.Value?.Errors.Select(x => x.ErrorMessage).ToArray());

            throw new BasicException(ResponseCode.ModelInvalid, errorInfos: errors);
        }

        await next();
    }
}
