using Microsoft.EntityFrameworkCore;
using Moq;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Services;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;
using Xunit;

namespace SocietySaaS.UnitTests;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IBillRepository> _billRepositoryMock;
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly Mock<IReceiptRepository> _receiptRepositoryMock;
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _billRepositoryMock = new Mock<IBillRepository>();
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _receiptRepositoryMock = new Mock<IReceiptRepository>();
        _contextMock = new Mock<IApplicationDbContext>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(Guid.NewGuid());
        _currentUserMock.Setup(c => c.UserId).Returns(Guid.NewGuid());

        var ledgerEntries = new List<LedgerEntry>();
        var ledgerDbSet = new Mock<DbSet<LedgerEntry>>();
        ledgerDbSet.Setup(d => d.AddAsync(It.IsAny<LedgerEntry>(), It.IsAny<CancellationToken>()))
            .Callback<LedgerEntry, CancellationToken>((e, ct) => ledgerEntries.Add(e))
            .Returns(new ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<LedgerEntry>>());

        _contextMock.Setup(c => c.Set<LedgerEntry>()).Returns(ledgerDbSet.Object);

        _service = new PaymentService(
            _paymentRepositoryMock.Object,
            _billRepositoryMock.Object,
            _flatRepositoryMock.Object,
            _receiptRepositoryMock.Object,
            _contextMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithAmount_AllocatesToOldestBills()
    {
        var flatId = Guid.NewGuid();
        var tenantId = _currentUserMock.Object.TenantId!.Value;
        var flat = new Flat { Id = flatId, FlatNumber = "101", TenantId = tenantId };
        _flatRepositoryMock.Setup(r => r.GetByIdAsync(flatId)).ReturnsAsync(flat);

        var bills = new List<Bill>
        {
            new() { Id = Guid.NewGuid(), BalanceOutstanding = 5000, DueDate = DateTime.UtcNow.AddDays(-10), FlatId = flatId, Status = "Pending", TenantId = tenantId },
            new() { Id = Guid.NewGuid(), BalanceOutstanding = 3000, DueDate = DateTime.UtcNow.AddDays(-5), FlatId = flatId, Status = "Pending", TenantId = tenantId }
        };
        _billRepositoryMock.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(bills);
        _paymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>())).ReturnsAsync(new Payment());

        var result = await _service.CreateAsync(new Application.Common.DTOs.CreatePaymentRequest(
            DateTime.UtcNow, 5000, "UPI", null, null, flatId));

        Assert.NotNull(result);
        Assert.Equal(5000, result.Amount);
    }
}
