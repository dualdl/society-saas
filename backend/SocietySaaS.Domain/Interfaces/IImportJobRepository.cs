using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Domain.Interfaces;

public interface IImportJobRepository : IRepository<ImportJob>
{
    Task<ImportJob?> GetWithRowsAsync(Guid id);
}
