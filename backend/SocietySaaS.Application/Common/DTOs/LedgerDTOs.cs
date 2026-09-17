namespace SocietySaaS.Application.Common.DTOs;

public record LedgerEntryDto
{
    public Guid Id { get; init; }
    public DateTime TransactionDate { get; init; }
    public string ReferenceType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Debit { get; init; }
    public decimal Credit { get; init; }
    public Guid FlatId { get; init; }
    public string FlatNumber { get; init; } = string.Empty;
}

public record TrialBalanceDto
{
    public decimal TotalDebit { get; init; }
    public decimal TotalCredit { get; init; }
    public bool IsBalanced { get; init; }
    public List<LedgerEntryDto> Entries { get; init; } = new();
}
