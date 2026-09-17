using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Address).HasMaxLength(500);
        builder.Property(t => t.City).HasMaxLength(200);
        builder.Property(t => t.State).HasMaxLength(100);
        builder.Property(t => t.PinCode).HasMaxLength(10);
        builder.Property(t => t.Phone).HasMaxLength(20);
        builder.Property(t => t.Email).HasMaxLength(100);
        builder.Property(t => t.LogoUrl).HasMaxLength(500);
        builder.Property(t => t.Slug).HasMaxLength(50);
        builder.Property(t => t.EmailProvider).HasMaxLength(50);
        builder.Property(t => t.SmtpHost).HasMaxLength(200);
        builder.Property(t => t.SmtpUser).HasMaxLength(200);
        builder.Property(t => t.SmtpPassword).HasMaxLength(200);
        builder.Property(t => t.GmailAddress).HasMaxLength(200);
        builder.Property(t => t.GmailAppPassword).HasMaxLength(200);
        builder.HasIndex(t => t.Slug).IsUnique();
    }
}
