using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PaymentNumber).IsRequired().HasMaxLength(50);
        builder.Property(p => p.PaymentMode).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Status).HasMaxLength(20);
        builder.HasIndex(p => new { p.TenantId, p.PaymentDate });
        builder.HasOne(p => p.Flat).WithMany(f => f.Payments).HasForeignKey(p => p.FlatId).OnDelete(DeleteBehavior.Restrict);
    }
}
