using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IChargeService
{
    Task<List<ChargeDto>> GetAllAsync();
    Task<ChargeDto?> GetByIdAsync(Guid id);
    Task<ChargeDto> CreateAsync(CreateChargeRequest request);
    Task<ChargeDto> UpdateAsync(Guid id, UpdateChargeRequest request);
    Task DeleteAsync(Guid id);
}

public class ChargeService : IChargeService
{
    private readonly IChargeRepository _chargeRepository;
    private readonly ICurrentUserService _currentUser;

    public ChargeService(IChargeRepository chargeRepository, ICurrentUserService currentUser)
    {
        _chargeRepository = chargeRepository;
        _currentUser = currentUser;
    }

    public async Task<List<ChargeDto>> GetAllAsync()
    {
        var charges = await _chargeRepository.GetActiveChargesAsync();
        var tenantId = _currentUser.TenantId;
        return charges
            .Where(c => c.TenantId == tenantId)
            .OrderBy(c => c.Name)
            .Select(c => new ChargeDto(c.Id, c.Name, c.Description, c.CalculationType, c.Amount, c.IsRecurring, c.IsActive))
            .ToList();
    }

    public async Task<ChargeDto?> GetByIdAsync(Guid id)
    {
        var charge = await _chargeRepository.GetByIdAsync(id);
        if (charge == null) return null;
        return new ChargeDto(charge.Id, charge.Name, charge.Description, charge.CalculationType, charge.Amount, charge.IsRecurring, charge.IsActive);
    }

    public async Task<ChargeDto> CreateAsync(CreateChargeRequest request)
    {
        var charge = new Charge
        {
            Name = request.Name,
            Description = request.Description,
            CalculationType = request.CalculationType,
            Amount = request.Amount,
            IsRecurring = request.IsRecurring,
            TenantId = _currentUser.TenantId!.Value
        };

        await _chargeRepository.AddAsync(charge);
        return new ChargeDto(charge.Id, charge.Name, charge.Description, charge.CalculationType, charge.Amount, charge.IsRecurring, charge.IsActive);
    }

    public async Task<ChargeDto> UpdateAsync(Guid id, UpdateChargeRequest request)
    {
        var charge = await _chargeRepository.GetByIdAsync(id);
        if (charge == null) throw new KeyNotFoundException("Charge not found");

        charge.Name = request.Name;
        charge.Description = request.Description;
        charge.CalculationType = request.CalculationType;
        charge.Amount = request.Amount;
        charge.IsRecurring = request.IsRecurring;
        charge.IsActive = request.IsActive;

        await _chargeRepository.UpdateAsync(charge);
        return new ChargeDto(charge.Id, charge.Name, charge.Description, charge.CalculationType, charge.Amount, charge.IsRecurring, charge.IsActive);
    }

    public async Task DeleteAsync(Guid id)
    {
        var charge = await _chargeRepository.GetByIdAsync(id);
        if (charge == null) throw new KeyNotFoundException("Charge not found");
        charge.IsDeleted = true;
        await _chargeRepository.UpdateAsync(charge);
    }
}
