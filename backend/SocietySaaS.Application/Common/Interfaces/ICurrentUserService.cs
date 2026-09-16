using System.Security.Claims;

namespace SocietySaaS.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    
    Guid? TenantId { get; }
    
    string? Email { get; }
    
    bool IsAuthenticated { get; }
    
    bool IsSuperAdmin { get; }
    
    ClaimsPrincipal? User { get; }
}
