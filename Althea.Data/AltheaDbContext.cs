using System.Reflection;

using Althea.Infrastructure.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

namespace Althea.Data;

public class AltheaDbContext : BasicDbContext
{
    public AltheaDbContext(DbContextOptions options, IAuditInfoProvider auditInfoProvider)
        : base(options, auditInfoProvider)
    {
    }

    protected override Assembly[] GetModelAssemblies()
    {
        return new[] { typeof(AltheaDbContext).Assembly };
    }
}
