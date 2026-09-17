using Microsoft.EntityFrameworkCore;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Infrastructure.Repositories;

public class ImportJobRepository : Repository<ImportJob>, IImportJobRepository
{
    public ImportJobRepository(IApplicationDbContext context) : base(context) { }

    public async Task<ImportJob?> GetWithRowsAsync(Guid id)
    {
        return await _context.Set<ImportJob>()
            .Include(ij => ij.ImportRows)
            .FirstOrDefaultAsync(ij => ij.Id == id);
    }
}
