using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IEmailTemplateRepository
{
    Task<EmailTemplate?> GetByNameAsync(string name);
    Task<IReadOnlyList<EmailTemplate>> GetAllAsync();
    Task AddAsync(EmailTemplate template);
    Task UpdateAsync(EmailTemplate template);
}
