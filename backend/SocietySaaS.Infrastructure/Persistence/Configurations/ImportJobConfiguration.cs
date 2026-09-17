using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class ImportJobConfiguration : IEntityTypeConfiguration<ImportJob>
{
    public void Configure(EntityTypeBuilder<ImportJob> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.JobType).IsRequired().HasMaxLength(50);
        builder.Property(i => i.FileName).IsRequired().HasMaxLength(500);
        builder.Property(i => i.Status).HasMaxLength(20);
    }
}
