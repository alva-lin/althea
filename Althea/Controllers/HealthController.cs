using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Althea.Controllers;

/// <summary>
///     健康检测控制器
/// </summary>
public class HealthController : BasicApiController
{
    public HealthController(ILogger<HealthController> logger)
        : base(logger)
    {
    }

    /// <summary>
    ///     心跳检测
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("[action]")]
    public IActionResult HeartBeat()
    {
        return Ok();
    }
}