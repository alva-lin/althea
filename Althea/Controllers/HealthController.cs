using Althea.Infrastructure.AspNetCore;

using Microsoft.AspNetCore.Mvc;

namespace Althea.Controllers;

/// <summary>
/// 健康检测控制器
/// </summary>
public class HealthController : BasicApiController
{
    public HealthController(ILogger<HealthController> logger)
        : base(logger)
    {

    }

    [HttpGet("[action]")]
    public IActionResult HeartBeat()
    {
        return Ok();
    }

    [HttpGet("[action]")]
    public IActionResult GetTime()
    {
        return Ok(new { DateTime.Now });
    }
}
