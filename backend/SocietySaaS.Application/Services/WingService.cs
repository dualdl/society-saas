using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IWingService
{
    Task<List<WingDto>> GetAllAsync();
    Task<WingDto?> GetByIdAsync(Guid id);
    Task<WingDto> CreateAsync(CreateWingRequest request);
    Task<WingDto> UpdateAsync(Guid id, UpdateWingRequest request);
    Task DeleteAsync(Guid id);
}

public class WingService : IWingService
{
    private readonly IWingRepository _wingRepository;
    private readonly ICurrentUserService _currentUser;

    public WingService(IWingRepository wingRepository, ICurrentUserService currentUser)
    {
        _wingRepository = wingRepository;
        _currentUser = currentUser;
    }

    public async Task<List<WingDto>> GetAllAsync()
    {
        var wings = await _wingRepository.GetAllActiveAsync();
        var tenantId = _currentUser.TenantId;
        return wings
            .Where(w => w.TenantId == tenantId)
            .OrderBy(w => w.Name)
            .Select(w => new WingDto(w.Id, w.Name, w.TotalFloors, w.FlatsPerFloor, w.IsActive))
            .ToList();
    }

    public async Task<WingDto?> GetByIdAsync(Guid id)
    {
        var wing = await _wingRepository.GetByIdAsync(id);
        if (wing == null) return null;
        return new WingDto(wing.Id, wing.Name, wing.TotalFloors, wing.FlatsPerFloor, wing.IsActive);
    }

    public async Task<WingDto> CreateAsync(CreateWingRequest request)
    {
        var wing = new Wing
        {
            Name = request.Name,
            TotalFloors = request.TotalFloors,
            FlatsPerFloor = request.FlatsPerFloor,
            TenantId = _currentUser.TenantId!.Value
        };

        await _wingRepository.AddAsync(wing);
        return new WingDto(wing.Id, wing.Name, wing.TotalFloors, wing.FlatsPerFloor, wing.IsActive);
    }

    public async Task<WingDto> UpdateAsync(Guid id, UpdateWingRequest request)
    {
        var wing = await _wingRepository.GetByIdAsync(id);
        if (wing == null) throw new KeyNotFoundException("Wing not found");

        wing.Name = request.Name;
        wing.TotalFloors = request.TotalFloors;
        wing.FlatsPerFloor = request.FlatsPerFloor;
        wing.IsActive = request.IsActive;

        await _wingRepository.UpdateAsync(wing);
        return new WingDto(wing.Id, wing.Name, wing.TotalFloors, wing.FlatsPerFloor, wing.IsActive);
    }

    public async Task DeleteAsync(Guid id)
    {
        var wing = await _wingRepository.GetByIdAsync(id);
        if (wing == null) throw new KeyNotFoundException("Wing not found");
        wing.IsDeleted = true;
        await _wingRepository.UpdateAsync(wing);
    }
}
