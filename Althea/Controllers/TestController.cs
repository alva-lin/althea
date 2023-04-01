using Althea.Data.Models;
using Althea.Infrastructure.AspNetCore;
using Althea.Services;

using Microsoft.AspNetCore.Mvc;

namespace Althea.Controllers;

/// <summary>
/// 测试接口
/// </summary>
public class TestController : BasicApiController
{
    public TestController(ILogger<TestController> logger, ITestService testService)
        : base(logger)
    {
        TestService = testService;
    }

    public ITestService TestService { get; set; }

    /// <summary>
    /// 获取所有数据
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetAll")]
    public async Task<ResponseResult<TestModel[]>> GetAll()
    {
        var models = await TestService.GetAsync();
        return models;
    }

    [HttpGet("{id}", Name = "Get")]
    public async Task<ResponseResult<TestModel?>> Get(long id)
    {
        var model = await TestService.GetAsync(id);
        return model;
    }

    [HttpPost(Name = "Add")]
    public async Task<ResponseResult<bool>> Add(TestModel model)
    {
        var result = await TestService.AddAsync(model);
        return result;
    }

    [HttpDelete("{id}", Name = "Delete")]
    public async Task<ResponseResult<bool>> Delete(long id)
    {
        var result = await TestService.DeleteAsync(id);
        return result;
    }

}
