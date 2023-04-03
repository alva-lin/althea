using System.Reflection;

using Althea.Infrastructure.EntityFrameworkCore.Entities;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedParameter.Local

#pragma warning disable CS8618

namespace Althea.Infrastructure.EntityFrameworkCore;

public interface IAuditInfoProvider
{
    string CurrentUser { get; }
}

public class UnknownAuditInfoProvider : IAuditInfoProvider
{
    public string CurrentUser => "Unknown";
}

public abstract class BasicDbContext : DbContext
{
    protected readonly IAuditInfoProvider AuditInfoProvider;

    protected readonly ILogger<BasicDbContext> Logger;

    protected BasicDbContext(DbContextOptions options, IAuditInfoProvider auditInfoProvider,
        ILogger<BasicDbContext> logger)
        : base(options)
    {
        AuditInfoProvider          =  auditInfoProvider;
        Logger                     =  logger;
        ChangeTracker.StateChanged += StateChangedAndTrackedEvent;
        ChangeTracker.Tracked      += StateChangedAndTrackedEvent;
    }

    /// <summary>
    /// 获取所有需要注册的实体配置程序集
    /// </summary>
    /// <returns></returns>
    protected abstract Assembly[] GetModelAssemblies();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .ConfigureBaseEntityTypes(GetModelAssemblies())
            .ApplyAuditInfo()
            .ApplyUtcDateTimeConverter()
            .AddDateOnlySupport()
            .AddTimeOnlySupport()
            ;

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BasicDbContext).Assembly);
    }

    #region Events

    protected void StateChangedAndTrackedEvent(object? sender, EntityEntryEventArgs e)
    {
        SetPartialUpdate(sender, e);
        SetAuditInfo(sender, e);
    }

    private void SetPartialUpdate(object? sender, EntityEntryEventArgs e)
    {
        if (e.Entry.State == EntityState.Modified)
        {
            foreach (var propertyEntry in e.Entry.Properties.Where(pEntry =>
                         pEntry.IsModified && pEntry.Metadata.Name != nameof(IAuditable.Audit)))
            {
                if (Equals(propertyEntry.OriginalValue, propertyEntry.CurrentValue))
                {
                    propertyEntry.IsModified = false;
                }
            }
            if (e.Entry.Properties.Count(pEntry => pEntry.IsModified) == 0)
            {
                e.Entry.State = EntityState.Unchanged;
            }
        }
    }

    private void SetAuditInfo(object? sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is IAuditable auditable)
        {
            switch (e.Entry.State)
            {
                case EntityState.Deleted:
                    var deletableAudit = (DeletableAudit)auditable.Audit;
                    deletableAudit.IsDeleted    = true;
                    deletableAudit.DeletedTime = DateTime.UtcNow;
                    deletableAudit.DeletedBy   = AuditInfoProvider.CurrentUser;

                    e.Entry.State = EntityState.Modified;
                    break;
                case EntityState.Modified:
                    var editableAudit = (EditableAudit)auditable.Audit;
                    editableAudit.ModifiedTime = DateTime.UtcNow;
                    editableAudit.ModifiedBy   = AuditInfoProvider.CurrentUser;

                    e.Entry.Property(nameof(IAuditable.Audit)).IsModified = true;
                    break;
                case EntityState.Added:
                    var basicAudit = (BasicAudit)auditable.Audit;
                    basicAudit.CreationTime = DateTime.UtcNow;
                    basicAudit.CreatedBy    = AuditInfoProvider.CurrentUser;
                    break;
            }
        }
    }

    #endregion
}
