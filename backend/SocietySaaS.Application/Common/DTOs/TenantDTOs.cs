namespace SocietySaaS.Application.Common.DTOs;

public record TenantDto(
    Guid Id, string Name, string? Address, string? City, string? State, string? PinCode,
    string? Phone, string? Email, bool IsActive, DateTime CreatedAt,
    string? LogoUrl, string? Slug,
    string? EmailProvider, string? SmtpHost, int? SmtpPort, string? SmtpUser, bool SmtpUseSsl,
    string? GmailAddress);

public record CreateTenantRequest(string Name, string? Address, string? City, string? State, string? PinCode, string? Phone, string? Email);
public record UpdateTenantRequest(string Name, string? Address, string? City, string? State, string? PinCode, string? Phone, string? Email, bool IsActive);

public record SocietySettingsDto(
    string Name, string? Address, string? City, string? State, string? PinCode,
    string? Phone, string? Email, string? LogoUrl, string? Slug,
    string? EmailProvider, string? SmtpHost, int? SmtpPort, string? SmtpUser, string? SmtpPassword, bool SmtpUseSsl,
    string? GmailAddress, string? GmailAppPassword);

public record UpdateSocietySettingsRequest(
    string Name, string? Address, string? City, string? State, string? PinCode,
    string? Phone, string? Email, string? LogoUrl,
    string? EmailProvider, string? SmtpHost, int? SmtpPort, string? SmtpUser, string? SmtpPassword, bool SmtpUseSsl,
    string? GmailAddress, string? GmailAppPassword);
