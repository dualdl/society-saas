using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Common.Models;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IFlatService
{
    Task<PaginatedList<FlatDto>> GetAllAsync(int page, int pageSize, string? search);
    Task<FlatDto?> GetByIdAsync(Guid id);
    Task<FlatDto> CreateAsync(CreateFlatRequest request);
    Task<FlatDto> UpdateAsync(Guid id, UpdateFlatRequest request);
    Task DeleteAsync(Guid id);
}

public class FlatService : IFlatService
{
    private readonly IFlatRepository _flatRepository;
    private readonly ICurrentUserService _currentUser;

    public FlatService(IFlatRepository flatRepository, ICurrentUserService currentUser)
    {
        _flatRepository = flatRepository;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<FlatDto>> GetAllAsync(int page, int pageSize, string? search)
    {
        var flats = await _flatRepository.GetAllWithMembersAsync();
        var tenantId = _currentUser.TenantId;

        var filtered = flats.Where(f => f.TenantId == tenantId);
        if (!string.IsNullOrEmpty(search))
            filtered = filtered.Where(f => f.FlatNumber.Contains(search, StringComparison.OrdinalIgnoreCase));

        var total = filtered.Count();
        var paged = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedList<FlatDto>(
            paged.Select(MapToDto).ToList(),
            total,
            page,
            pageSize);
    }

    public async Task<FlatDto?> GetByIdAsync(Guid id)
    {
        var flat = await _flatRepository.GetWithDetailsAsync(id);
        if (flat == null) return null;
        return MapToDto(flat);
    }

    public async Task<FlatDto> CreateAsync(CreateFlatRequest request)
    {
        var flat = new Flat
        {
            FlatNumber = request.FlatNumber,
            Floor = request.Floor,
            CarpetArea = request.CarpetArea,
            BuiltUpArea = request.BuiltUpArea,
            FlatType = request.FlatType,
            OccupancyStatus = request.OccupancyStatus,
            WingId = request.WingId,
            TenantId = _currentUser.TenantId!.Value
        };

        await _flatRepository.AddAsync(flat);
        return MapToDto(flat);
    }

    public async Task<FlatDto> UpdateAsync(Guid id, UpdateFlatRequest request)
    {
        var flat = await _flatRepository.GetByIdAsync(id);
        if (flat == null) throw new KeyNotFoundException("Flat not found");

        flat.FlatNumber = request.FlatNumber;
        flat.Floor = request.Floor;
        flat.CarpetArea = request.CarpetArea;
        flat.BuiltUpArea = request.BuiltUpArea;
        flat.FlatType = request.FlatType;
        flat.OccupancyStatus = request.OccupancyStatus;
        flat.WingId = request.WingId;
        flat.IsActive = request.IsActive;

        await _flatRepository.UpdateAsync(flat);
        return MapToDto(flat);
    }

    public async Task DeleteAsync(Guid id)
    {
        var flat = await _flatRepository.GetByIdAsync(id);
        if (flat == null) throw new KeyNotFoundException("Flat not found");
        flat.IsDeleted = true;
        await _flatRepository.UpdateAsync(flat);
    }

    private static FlatDto MapToDto(Flat flat)
    {
        return new FlatDto(
            flat.Id,
            flat.FlatNumber,
            flat.Floor,
            flat.CarpetArea,
            flat.BuiltUpArea,
            flat.FlatType,
            flat.OccupancyStatus,
            flat.IsActive,
            flat.WingId,
            flat.Wing?.Name,
            flat.Members?.Count(m => !m.IsDeleted) ?? 0,
            flat.Bills?.Where(b => !b.IsDeleted).Sum(b => b.BalanceOutstanding) ?? 0);
    }
}
