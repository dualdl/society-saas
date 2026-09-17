using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class OpeningBalanceConfiguration : IEntityTypeConfiguration<OpeningBalance>
{
    public void Configure(EntityTypeBuilder<OpeningBalance> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasIndex(o => o.FlatId).IsUnique();
        builder.Property(o => o.BalanceType).HasMaxLength(20);
    }
}
