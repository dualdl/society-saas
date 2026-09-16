using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SocietySaaS.Application.Common.Interfaces;

namespace SocietySaaS.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        }
    }
    
    public Guid? TenantId
    {
        get
        {
            var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId");
            return tenantIdClaim != null ? Guid.Parse(tenantIdClaim.Value) : null;
        }
    }
    
    public string? Email
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
    
    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
    
    public bool IsSuperAdmin
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole("SuperAdmin") ?? false;
        }
    }
    
    public ClaimsPrincipal? User
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User;
        }
    }
}
