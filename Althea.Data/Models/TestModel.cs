using Althea.Infrastructure.EntityFrameworkCore;
using Althea.Infrastructure.EntityFrameworkCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Althea.Data.Models;

public class TestModel : DeletableEntity<long>
{

}

public class TestModelConfiguration : BasicEntityConfiguration<TestModel, long>
{
    public override void Configure(EntityTypeBuilder<TestModel> builder)
    {
        base.Configure(builder);

        builder.ToTable("Test");
    }
}
