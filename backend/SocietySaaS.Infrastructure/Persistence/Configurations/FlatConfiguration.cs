using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class FlatConfiguration : IEntityTypeConfiguration<Flat>
{
    public void Configure(EntityTypeBuilder<Flat> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.FlatNumber).IsRequired().HasMaxLength(20);
        builder.Property(f => f.FlatType).HasMaxLength(20);
        builder.Property(f => f.OccupancyStatus).HasMaxLength(20);
        builder.HasIndex(f => new { f.TenantId, f.FlatNumber });
        builder.HasOne(f => f.Wing).WithMany(w => w.Flats).HasForeignKey(f => f.WingId).IsRequired(false);
    }
}
