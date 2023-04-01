using Althea.Infrastructure.EntityFrameworkCore;
using Althea.Infrastructure.EntityFrameworkCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Althea.Data.Models;

/// <summary>
/// 测试数据模型
/// </summary>
public class TestModel : DeletableEntity<long>
{

}

public class TestModelConfiguration : BasicEntityWithAuditConfiguration<TestModel, long, DeletableAudit>
{
    public override void Configure(EntityTypeBuilder<TestModel> builder)
    {
        base.Configure(builder);

        builder.ToTable("Test");
    }
}
