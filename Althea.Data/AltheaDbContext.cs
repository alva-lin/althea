using System.Reflection;

namespace Althea.Data;

public class AltheaDbContext : BasicDbContext
{
    public AltheaDbContext(DbContextOptions options, IAuthInfoProvider authInfoProvider,
        ILogger<AltheaDbContext> logger)
        : base(options, authInfoProvider, logger)
    {
    }

    protected override Assembly[] GetModelAssemblies()
    {
        return new[] { typeof(AltheaDbContext).Assembly };
    }
}