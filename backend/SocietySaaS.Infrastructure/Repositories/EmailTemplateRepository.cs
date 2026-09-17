using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class EmailTemplateRepository : IEmailTemplateRepository
{
    private readonly IApplicationDbContext _context;

    public EmailTemplateRepository(IApplicationDbContext context) => _context = context;

    public async Task<EmailTemplate?> GetByNameAsync(string name)
    {
        return await _context.Set<EmailTemplate>().FirstOrDefaultAsync(t => t.Name == name && t.IsActive);
    }

    public async Task<IReadOnlyList<EmailTemplate>> GetAllAsync()
    {
        return await _context.Set<EmailTemplate>().Where(t => t.IsActive).ToListAsync();
    }

    public async Task AddAsync(EmailTemplate template)
    {
        await _context.Set<EmailTemplate>().AddAsync(template);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EmailTemplate template)
    {
        _context.Set<EmailTemplate>().Update(template);
        await _context.SaveChangesAsync();
    }
}
