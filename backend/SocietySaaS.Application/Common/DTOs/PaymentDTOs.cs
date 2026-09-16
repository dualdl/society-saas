namespace SocietySaaS.Application.Common.DTOs;

public record PaymentDto(Guid Id, string PaymentNumber, DateTime PaymentDate, decimal Amount, string PaymentMode, string? TransactionReference, string? Notes, string Status, Guid FlatId, string? FlatNumber);
public record CreatePaymentRequest(DateTime PaymentDate, decimal Amount, string PaymentMode, string? TransactionReference, string? Notes, Guid FlatId);
public record ReversePaymentRequest(string ReversalReason);
public record PaymentAllocationDto(Guid Id, decimal Amount, Guid PaymentId, Guid BillId, string? BillNumber);
