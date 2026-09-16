using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Common;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Tenant> Tenants => Set<Tenant>();
    
    public DbSet<User> Users => Set<User>();
    
    public DbSet<UserTenant> UserTenants => Set<UserTenant>();
    
    public DbSet<Wing> Wings => Set<Wing>();
    
    public DbSet<Flat> Flats => Set<Flat>();
    
    public DbSet<Member> Members => Set<Member>();
    
    public DbSet<Charge> Charges => Set<Charge>();
    
    public DbSet<Bill> Bills => Set<Bill>();
    
    public DbSet<BillLine> BillLines => Set<BillLine>();
    
    public DbSet<Payment> Payments => Set<Payment>();
    
    public DbSet<PaymentAllocation> PaymentAllocations => Set<PaymentAllocation>();
    
    public DbSet<Receipt> Receipts => Set<Receipt>();
    
    public DbSet<OpeningBalance> OpeningBalances => Set<OpeningBalance>();
    
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    
    public DbSet<EmailQueue> EmailQueue => Set<EmailQueue>();
    
    public DbSet<ImportJob> ImportJobs => Set<ImportJob>();
    
    public DbSet<ImportRow> ImportRows => Set<ImportRow>();
    
    public DbSet<Document> Documents => Set<Document>();
    
    public DbSet<LatePaymentRule> LatePaymentRules => Set<LatePaymentRule>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}
