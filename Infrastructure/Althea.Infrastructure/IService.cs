using Microsoft.Extensions.Logging;

namespace Althea.Infrastructure;

public interface IService
{
}

public abstract class BasicService : IService
{
    protected readonly ILogger<BasicService> Logger;

    protected BasicService(ILogger<BasicService> logger)
    {
        Logger = logger;
    }
}
