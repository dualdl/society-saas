using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class LatePaymentRuleConfiguration : IEntityTypeConfiguration<LatePaymentRule>
{
    public void Configure(EntityTypeBuilder<LatePaymentRule> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.CalculationType).HasMaxLength(20);
        builder.Property(l => l.Frequency).HasMaxLength(20);
        builder.HasIndex(l => l.TenantId).IsUnique();
    }
}
