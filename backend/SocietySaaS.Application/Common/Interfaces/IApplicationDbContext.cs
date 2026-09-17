using Microsoft.EntityFrameworkCore;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    
    DbSet<User> Users { get; }
    
    DbSet<UserTenant> UserTenants { get; }
    
    DbSet<Wing> Wings { get; }
    
    DbSet<Flat> Flats { get; }
    
    DbSet<Member> Members { get; }
    
    DbSet<Charge> Charges { get; }
    
    DbSet<Bill> Bills { get; }
    
    DbSet<BillLine> BillLines { get; }
    
    DbSet<Payment> Payments { get; }
    
    DbSet<PaymentAllocation> PaymentAllocations { get; }
    
    DbSet<Receipt> Receipts { get; }
    
    DbSet<OpeningBalance> OpeningBalances { get; }
    
    DbSet<AuditLog> AuditLogs { get; }
    
    DbSet<EmailTemplate> EmailTemplates { get; }
    
    DbSet<EmailQueue> EmailQueue { get; }
    
    DbSet<ImportJob> ImportJobs { get; }
    
    DbSet<ImportRow> ImportRows { get; }
    
    DbSet<Document> Documents { get; }
    
    DbSet<LatePaymentRule> LatePaymentRules { get; }
    
    DbSet<OtpRequest> OtpRequests { get; }
    
    DbSet<RefreshToken> RefreshTokens { get; }
    
    DbSet<LedgerEntry> LedgerEntries { get; }
    
    DbSet<Account> Accounts { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    DbSet<T> Set<T>() where T : class;
}
