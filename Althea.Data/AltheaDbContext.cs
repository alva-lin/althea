using System.Reflection;

using Microsoft.Extensions.Logging;

namespace Althea.Data;

public class AltheaDbContext : BasicDbContext
{
    public AltheaDbContext(DbContextOptions options, IAuditInfoProvider auditInfoProvider,
        ILogger<AltheaDbContext> logger)
        : base(options, auditInfoProvider, logger)
    {
    }

    protected override Assembly[] GetModelAssemblies()
    {
        return new[] { typeof(AltheaDbContext).Assembly };
    }
}
