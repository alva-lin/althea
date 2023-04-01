using Althea.Data;
using Althea.Data.Models;
using Althea.Infrastructure;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

namespace Althea.Services;

public class TestProfile : Profile
{
    public TestProfile()
    {
    }
}

public interface ITestService : IService
{
    Task<bool> AddAsync(TestModel model);

    Task<bool> DeleteAsync(long id);

    Task<TestModel?> GetAsync(long id);

    Task<TestModel[]> GetAsync();
}

[LifeScope(LifeScope.Scope, typeof(ITestService))]
public class TestService : BasicService, ITestService
{
    public TestService(ILogger<TestService> logger, AltheaDbContext dbContext)
        : base(logger)
    {
        DbContext = dbContext;
    }

    public AltheaDbContext DbContext { get; set; }

    public async Task<bool> AddAsync(TestModel model)
    {
        await DbContext.Set<TestModel>().AddAsync(model);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var model = await GetAsync(id);
        if (model is not null)
        {
            DbContext.Set<TestModel>().Remove(model);
        }
        return await DbContext.SaveChangesAsync() > 0;
    }

    public Task<TestModel?> GetAsync(long id)
    {
        return DbContext.Set<TestModel>().FindAsync(id).AsTask();
    }

    public async Task<TestModel[]> GetAsync()
    {
        return await DbContext.Set<TestModel>().ToArrayAsync();
    }
}
