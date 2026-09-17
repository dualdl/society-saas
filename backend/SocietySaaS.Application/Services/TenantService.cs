using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface ITenantService
{
    Task<List<TenantDto>> GetAllAsync();
    Task<TenantDto?> GetByIdAsync(Guid id);
    Task<object> CreateAsync(CreateTenantRequest request);
    Task<TenantDto> UpdateAsync(Guid id, UpdateTenantRequest request);
    Task DeleteAsync(Guid id);
    Task<SocietySettingsDto?> GetSettingsAsync(Guid tenantId);
    Task<SocietySettingsDto> UpdateSettingsAsync(Guid tenantId, UpdateSocietySettingsRequest request);
}

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUser;

    public TenantService(ITenantRepository tenantRepository, ICurrentUserService currentUser)
    {
        _tenantRepository = tenantRepository;
        _currentUser = currentUser;
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        var tenants = await _tenantRepository.GetAllAsync();
        return tenants
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null) return null;
        return MapToDto(tenant);
    }

    public async Task<object> CreateAsync(CreateTenantRequest request)
    {
        var slug = request.Name?.ToLower().Replace(" ", "").Replace("'", "") ?? "";
        var tenant = new Tenant
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            State = request.State,
            PinCode = request.PinCode,
            Phone = request.Phone,
            Email = request.Email,
            Slug = slug,
            IsActive = true
        };

        await _tenantRepository.AddAsync(tenant);

        return new { tenant.Id, tenant.Name, tenant.Slug };
    }

    public async Task<TenantDto> UpdateAsync(Guid id, UpdateTenantRequest request)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null) throw new KeyNotFoundException("Tenant not found");

        tenant.Name = request.Name;
        tenant.Address = request.Address;
        tenant.City = request.City;
        tenant.State = request.State;
        tenant.PinCode = request.PinCode;
        tenant.Phone = request.Phone;
        tenant.Email = request.Email;
        tenant.IsActive = request.IsActive;

        await _tenantRepository.UpdateAsync(tenant);
        return MapToDto(tenant);
    }

    public async Task DeleteAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        if (tenant == null) throw new KeyNotFoundException("Tenant not found");
        tenant.IsDeleted = true;
        await _tenantRepository.UpdateAsync(tenant);
    }

    public async Task<SocietySettingsDto?> GetSettingsAsync(Guid tenantId)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null) return null;
        return new SocietySettingsDto(
            tenant.Name, tenant.Address, tenant.City, tenant.State, tenant.PinCode,
            tenant.Phone, tenant.Email, tenant.LogoUrl, tenant.Slug,
            tenant.EmailProvider, tenant.SmtpHost, tenant.SmtpPort, tenant.SmtpUser, null, tenant.SmtpUseSsl,
            tenant.GmailAddress, null);
    }

    public async Task<SocietySettingsDto> UpdateSettingsAsync(Guid tenantId, UpdateSocietySettingsRequest request)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null) throw new KeyNotFoundException("Tenant not found");

        tenant.Name = request.Name;
        tenant.Address = request.Address;
        tenant.City = request.City;
        tenant.State = request.State;
        tenant.PinCode = request.PinCode;
        tenant.Phone = request.Phone;
        tenant.Email = request.Email;
        tenant.LogoUrl = request.LogoUrl;
        tenant.EmailProvider = request.EmailProvider;
        tenant.SmtpHost = request.SmtpHost;
        tenant.SmtpPort = request.SmtpPort;
        tenant.SmtpUser = request.SmtpUser;
        if (!string.IsNullOrEmpty(request.SmtpPassword))
            tenant.SmtpPassword = request.SmtpPassword;
        tenant.SmtpUseSsl = request.SmtpUseSsl;
        tenant.GmailAddress = request.GmailAddress;
        if (!string.IsNullOrEmpty(request.GmailAppPassword))
            tenant.GmailAppPassword = request.GmailAppPassword;

        await _tenantRepository.UpdateAsync(tenant);
        return new SocietySettingsDto(
            tenant.Name, tenant.Address, tenant.City, tenant.State, tenant.PinCode,
            tenant.Phone, tenant.Email, tenant.LogoUrl, tenant.Slug,
            tenant.EmailProvider, tenant.SmtpHost, tenant.SmtpPort, tenant.SmtpUser, null, tenant.SmtpUseSsl,
            tenant.GmailAddress, null);
    }

    private static TenantDto MapToDto(Tenant t)
    {
        return new TenantDto(t.Id, t.Name, t.Address, t.City, t.State, t.PinCode,
            t.Phone, t.Email, t.IsActive, t.CreatedAt,
            t.LogoUrl, t.Slug,
            t.EmailProvider, t.SmtpHost, t.SmtpPort, t.SmtpUser, t.SmtpUseSsl,
            t.GmailAddress);
    }
}
