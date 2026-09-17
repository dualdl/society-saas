using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user, Guid? tenantId = null);
}
