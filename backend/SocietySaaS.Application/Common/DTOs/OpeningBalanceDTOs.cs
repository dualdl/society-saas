namespace SocietySaaS.Application.Common.DTOs;

public record OpeningBalanceDto
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public string BalanceType { get; init; } = string.Empty;
    public DateTime AsOfDate { get; init; }
    public string? Notes { get; init; }
    public Guid FlatId { get; init; }
}

public record SetOpeningBalanceRequest
{
    public decimal Amount { get; init; }
    public string BalanceType { get; init; } = "Debit";
    public DateTime AsOfDate { get; init; }
    public string? Notes { get; init; }
    public Guid FlatId { get; init; }
}
