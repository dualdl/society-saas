using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(m => m.LastName).HasMaxLength(100);
        builder.Property(m => m.Mobile).HasMaxLength(20);
        builder.Property(m => m.Email).HasMaxLength(200);
        builder.Property(m => m.MemberType).HasMaxLength(20);
        builder.HasIndex(m => new { m.TenantId, m.FlatId });
        builder.HasOne(m => m.Flat).WithMany(f => f.Members).HasForeignKey(m => m.FlatId).OnDelete(DeleteBehavior.Restrict);
    }
}
