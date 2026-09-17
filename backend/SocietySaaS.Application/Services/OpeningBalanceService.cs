using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IOpeningBalanceService
{
    Task<List<OpeningBalanceDto>> GetAllAsync();
    Task<OpeningBalanceDto?> GetByFlatAsync(Guid flatId);
    Task<OpeningBalanceDto> SetAsync(SetOpeningBalanceRequest request);
}

public class OpeningBalanceService : IOpeningBalanceService
{
    private readonly IOpeningBalanceRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public OpeningBalanceService(IOpeningBalanceRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<List<OpeningBalanceDto>> GetAllAsync()
    {
        var balances = await _repository.GetAllWithFlatAsync();
        return balances.Where(b => b.TenantId == _currentUser.TenantId!.Value).Select(b => new OpeningBalanceDto
        {
            Id = b.Id, Amount = b.Amount, BalanceType = b.BalanceType,
            AsOfDate = b.AsOfDate, Notes = b.Notes, FlatId = b.FlatId
        }).ToList();
    }

    public async Task<OpeningBalanceDto?> GetByFlatAsync(Guid flatId)
    {
        var balance = await _repository.GetByFlatAsync(flatId);
        if (balance == null) return null;
        return new OpeningBalanceDto
        {
            Id = balance.Id, Amount = balance.Amount, BalanceType = balance.BalanceType,
            AsOfDate = balance.AsOfDate, Notes = balance.Notes, FlatId = balance.FlatId
        };
    }

    public async Task<OpeningBalanceDto> SetAsync(SetOpeningBalanceRequest request)
    {
        var existing = await _repository.GetByFlatAsync(request.FlatId);
        if (existing != null)
        {
            existing.Amount = request.Amount;
            existing.BalanceType = request.BalanceType;
            existing.AsOfDate = request.AsOfDate;
            existing.Notes = request.Notes;
            await _repository.UpdateAsync(existing);
            return new OpeningBalanceDto
            {
                Id = existing.Id, Amount = existing.Amount, BalanceType = existing.BalanceType,
                AsOfDate = existing.AsOfDate, Notes = existing.Notes, FlatId = existing.FlatId
            };
        }

        var balance = new OpeningBalance
        {
            Amount = request.Amount, BalanceType = request.BalanceType,
            AsOfDate = request.AsOfDate, Notes = request.Notes,
            FlatId = request.FlatId, TenantId = _currentUser.TenantId!.Value
        };
        await _repository.AddAsync(balance);
        return new OpeningBalanceDto
        {
            Id = balance.Id, Amount = balance.Amount, BalanceType = balance.BalanceType,
            AsOfDate = balance.AsOfDate, Notes = balance.Notes, FlatId = balance.FlatId
        };
    }
}
