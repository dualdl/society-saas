using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SocietySaaS.Domain.Entities;

namespace SocietySaaS.Infrastructure.Services;

public interface IReceiptPdfService
{
    byte[] GenerateReceiptPdf(Receipt receipt, string societyName, string societyAddress);
}

public class ReceiptPdfService : IReceiptPdfService
{
    public byte[] GenerateReceiptPdf(Receipt receipt, string societyName, string societyAddress)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(societyName).FontSize(20).Bold();
                            col.Item().Text(societyAddress).FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                        row.RelativeItem().AlignRight().Text("RECEIPT").FontSize(16).Bold();
                    });
                });

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        col.Item().PaddingVertical(10);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Receipt No: {receipt.ReceiptNumber}").Bold();
                            row.RelativeItem().AlignRight().Text($"Date: {receipt.ReceiptDate:dd-MMM-yyyy}");
                        });

                        col.Item().PaddingVertical(5);
                        col.Item().Text($"Received From: Flat {receipt.Flat?.FlatNumber ?? "N/A"}").FontSize(12);
                        col.Item().PaddingVertical(5);
                        col.Item().Text($"Amount: {receipt.Amount:N2}").FontSize(14).Bold();
                        col.Item().PaddingVertical(5);
                        col.Item().Text($"Payment Mode: {receipt.PaymentMode}");
                        col.Item().Text($"Transaction Reference: {receipt.TransactionReference ?? "N/A"}");
                        col.Item().PaddingVertical(10);
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        col.Item().PaddingVertical(10);
                        col.Item().Text("Authorized By").Italic();
                    });
                });

                page.Footer().Element(footer =>
                {
                    footer.AlignCenter().Text("Thank you for your payment!").FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }
}
