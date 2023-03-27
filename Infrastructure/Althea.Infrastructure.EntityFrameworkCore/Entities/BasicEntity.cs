using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618
namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

public abstract class BasicEntity<TKey> : IBasicEntity<TKey>, IAuditable<BasicAudit> where TKey : IEquatable<TKey>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TKey Id { get; set; }

    public BasicAudit Audit { get; set; }
}
