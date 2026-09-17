using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.ReceiptNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(r => new { r.TenantId, r.ReceiptDate });
    }
}
