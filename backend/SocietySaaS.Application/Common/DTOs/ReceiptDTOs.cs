namespace SocietySaaS.Application.Common.DTOs;

public record ReceiptDto(Guid Id, string ReceiptNumber, DateTime ReceiptDate, decimal Amount, string PaymentMode, string? TransactionReference, string? PdfUrl, Guid PaymentId, Guid FlatId, string? FlatNumber);
