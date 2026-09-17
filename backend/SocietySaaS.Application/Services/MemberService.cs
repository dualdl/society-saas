using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Common.Models;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;

namespace SocietySaaS.Application.Services;

public interface IMemberService
{
    Task<PaginatedList<MemberDto>> GetAllAsync(int page, int pageSize, string? search, Guid? flatId);
    Task<MemberDto?> GetByIdAsync(Guid id);
    Task<MemberDto> CreateAsync(CreateMemberRequest request);
    Task<MemberDto> UpdateAsync(Guid id, UpdateMemberRequest request);
    Task DeleteAsync(Guid id);
}

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ICurrentUserService _currentUser;

    public MemberService(IMemberRepository memberRepository, ICurrentUserService currentUser)
    {
        _memberRepository = memberRepository;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<MemberDto>> GetAllAsync(int page, int pageSize, string? search, Guid? flatId)
    {
        var members = await _memberRepository.GetAllWithFlatAsync();
        var tenantId = _currentUser.TenantId;

        var filtered = members.Where(m => m.TenantId == tenantId);
        if (!string.IsNullOrEmpty(search))
            filtered = filtered.Where(m =>
                m.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (m.LastName != null && m.LastName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                m.Mobile.Contains(search));
        if (flatId.HasValue)
            filtered = filtered.Where(m => m.FlatId == flatId.Value);

        var total = filtered.Count();
        var paged = filtered
            .OrderBy(m => m.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedList<MemberDto>(
            paged.Select(MapToDto).ToList(),
            total, page, pageSize);
    }

    public async Task<MemberDto?> GetByIdAsync(Guid id)
    {
        var member = await _memberRepository.GetWithFlatAsync(id);
        if (member == null) return null;
        return MapToDto(member);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberRequest request)
    {
        var member = new Member
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Mobile = request.Mobile,
            Email = request.Email,
            MemberType = request.MemberType,
            IsPrimary = request.IsPrimary,
            FlatId = request.FlatId,
            TenantId = _currentUser.TenantId!.Value
        };

        await _memberRepository.AddAsync(member);
        return MapToDto(member);
    }

    public async Task<MemberDto> UpdateAsync(Guid id, UpdateMemberRequest request)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null) throw new KeyNotFoundException("Member not found");

        member.FirstName = request.FirstName;
        member.LastName = request.LastName;
        member.Mobile = request.Mobile;
        member.Email = request.Email;
        member.MemberType = request.MemberType;
        member.IsPrimary = request.IsPrimary;
        member.IsActive = request.IsActive;

        await _memberRepository.UpdateAsync(member);
        return MapToDto(member);
    }

    public async Task DeleteAsync(Guid id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null) throw new KeyNotFoundException("Member not found");
        member.IsDeleted = true;
        await _memberRepository.UpdateAsync(member);
    }

    private static MemberDto MapToDto(Member member)
    {
        return new MemberDto(
            member.Id,
            member.FirstName,
            member.LastName,
            member.Mobile,
            member.Email,
            member.MemberType,
            member.IsPrimary,
            member.IsActive,
            member.FlatId,
            member.Flat?.FlatNumber);
    }
}
