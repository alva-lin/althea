﻿using Althea.Infrastructure.EntityFrameworkCore.Entities;

using Microsoft.EntityFrameworkCore.ChangeTracking;

// ReSharper disable UnusedParameter.Local

#pragma warning disable CS8618

namespace Althea.Infrastructure.EntityFrameworkCore;

public interface IAuditInfoProvider
{
    string CurrentUser { get; }
}

public class BasicDbContext : DbContext
{
    protected readonly IAuditInfoProvider AuditInfoProvider;

    public BasicDbContext(DbContextOptions options, IAuditInfoProvider auditInfoProvider)
        : base(options)
    {
        AuditInfoProvider          =  auditInfoProvider;
        ChangeTracker.StateChanged += StateChangedAndTrackedEvent;
        ChangeTracker.Tracked      += StateChangedAndTrackedEvent;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
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
            foreach (var propertyEntry in e.Entry.Properties.Where(pEntry => pEntry.IsModified))
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
        switch (e.Entry.State)
        {
            case EntityState.Deleted:
                if (e.Entry.Entity is IAuditable<DeletableAudit> deletableAudit)
                {
                    deletableAudit.Audit.IsDelete    = true;
                    deletableAudit.Audit.DeletedTime = DateTime.UtcNow;
                    deletableAudit.Audit.DeletedBy   = AuditInfoProvider.CurrentUser;
                }
                break;
            case EntityState.Modified:
                if (e.Entry.Entity is IAuditable<EditableAudit> editableAudit)
                {
                    editableAudit.Audit.ModifiedTime = DateTime.UtcNow;
                    editableAudit.Audit.ModifiedBy   = AuditInfoProvider.CurrentUser;
                }
                break;
            case EntityState.Added:
                if (e.Entry.Entity is IAuditable<BasicAudit> basicAudit)
                {
                    basicAudit.Audit = new()
                    {
                        CreationTime = DateTime.UtcNow,
                        CreatedBy    = AuditInfoProvider.CurrentUser
                    };
                }
                break;
        }
    }

    #endregion
}
