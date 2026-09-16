namespace SocietySaaS.Application.Common.DTOs;

public record BillDto(Guid Id, string BillNumber, string BillingPeriod, DateTime BillDate, DateTime DueDate, decimal PreviousOutstanding, decimal CurrentCharges, decimal LateFee, decimal Adjustment, decimal GrandTotal, decimal AmountPaid, decimal BalanceOutstanding, string Status, Guid FlatId, string? FlatNumber, List<BillLineDto> BillLines);
public record BillLineDto(Guid Id, decimal Amount, string? Description, Guid ChargeId, string? ChargeName);
public record GenerateBillRequest(string BillingPeriod, DateTime DueDate, Guid FlatId, List<BillLineRequest> BillLines);
public record BillLineRequest(Guid ChargeId, decimal Amount, string? Description);
public record BulkGenerateBillsRequest(string BillingPeriod, DateTime DueDate, List<Guid>? FlatIds);
