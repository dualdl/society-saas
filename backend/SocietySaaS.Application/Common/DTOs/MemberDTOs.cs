namespace SocietySaaS.Application.Common.DTOs;

public record MemberDto(Guid Id, string FirstName, string? LastName, string Mobile, string? Email, string MemberType, bool IsPrimary, bool IsActive, Guid FlatId, string? FlatNumber);
public record CreateMemberRequest(string FirstName, string? LastName, string Mobile, string? Email, string MemberType, bool IsPrimary, Guid FlatId);
public record UpdateMemberRequest(string FirstName, string? LastName, string Mobile, string? Email, string MemberType, bool IsPrimary, bool IsActive);
