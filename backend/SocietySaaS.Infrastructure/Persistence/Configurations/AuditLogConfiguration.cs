using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Module).HasMaxLength(50);
        builder.Property(a => a.EntityType).HasMaxLength(100);
        builder.HasIndex(a => new { a.TenantId, a.Timestamp });
        builder.HasIndex(a => new { a.EntityType, a.EntityId });
    }
}
