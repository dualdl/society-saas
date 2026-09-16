using Microsoft.EntityFrameworkCore;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; set; }
    
    DbSet<User> Users { get; set; }
    
    DbSet<UserTenant> UserTenants { get; set; }
    
    DbSet<Wing> Wings { get; set; }
    
    DbSet<Flat> Flats { get; set; }
    
    DbSet<Member> Members { get; set; }
    
    DbSet<Charge> Charges { get; set; }
    
    DbSet<Bill> Bills { get; set; }
    
    DbSet<BillLine> BillLines { get; set; }
    
    DbSet<Payment> Payments { get; set; }
    
    DbSet<PaymentAllocation> PaymentAllocations { get; set; }
    
    DbSet<Receipt> Receipts { get; set; }
    
    DbSet<OpeningBalance> OpeningBalances { get; set; }
    
    DbSet<AuditLog> AuditLogs { get; set; }
    
    DbSet<EmailTemplate> EmailTemplates { get; set; }
    
    DbSet<EmailQueue> EmailQueue { get; set; }
    
    DbSet<ImportJob> ImportJobs { get; set; }
    
    DbSet<ImportRow> ImportRows { get; set; }
    
    DbSet<Document> Documents { get; set; }
    
    DbSet<LatePaymentRule> LatePaymentRules { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
