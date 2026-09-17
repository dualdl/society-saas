using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.BillNumber).IsRequired().HasMaxLength(50);
        builder.Property(b => b.BillingPeriod).IsRequired().HasMaxLength(20);
        builder.Property(b => b.Status).HasMaxLength(20);
        builder.HasIndex(b => new { b.TenantId, b.BillingPeriod });
        builder.HasIndex(b => new { b.TenantId, b.FlatId });
    }
}
