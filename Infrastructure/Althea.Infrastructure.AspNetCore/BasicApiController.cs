using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Althea.Infrastructure.AspNetCore;

[ApiController]
[Route("api/[controller]")]
public abstract class BasicApiController : ControllerBase
{
    protected BasicApiController(ILogger<BasicApiController> logger)
    {
        _logger = logger;
    }

    protected readonly ILogger<BasicApiController> _logger;
}
