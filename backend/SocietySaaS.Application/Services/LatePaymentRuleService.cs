using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface ILatePaymentRuleService
{
    Task<LatePaymentRule?> GetRuleAsync();
    Task<LatePaymentRule> CreateOrUpdateAsync(decimal percentage, int gracePeriodDays, string calculationType, decimal? maximumFine, bool isEnabled);
}

public class LatePaymentRuleService : ILatePaymentRuleService
{
    private readonly ILatePaymentRuleRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public LatePaymentRuleService(ILatePaymentRuleRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<LatePaymentRule?> GetRuleAsync()
    {
        return await _repository.GetForTenantAsync(_currentUser.TenantId!.Value);
    }

    public async Task<LatePaymentRule> CreateOrUpdateAsync(decimal percentage, int gracePeriodDays, string calculationType, decimal? maximumFine, bool isEnabled)
    {
        var existing = await _repository.GetForTenantAsync(_currentUser.TenantId!.Value);
        if (existing != null)
        {
            existing.Percentage = percentage;
            existing.GracePeriodDays = gracePeriodDays;
            existing.CalculationType = calculationType;
            existing.MaximumFine = maximumFine;
            existing.IsEnabled = isEnabled;
            await _repository.UpdateAsync(existing);
            return existing;
        }

        var rule = new LatePaymentRule
        {
            Percentage = percentage, GracePeriodDays = gracePeriodDays,
            CalculationType = calculationType, MaximumFine = maximumFine,
            IsEnabled = isEnabled, TenantId = _currentUser.TenantId!.Value
        };
        await _repository.AddAsync(rule);
        return rule;
    }
}
