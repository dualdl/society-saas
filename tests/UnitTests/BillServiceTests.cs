using Microsoft.EntityFrameworkCore;
using Moq;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Services;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;
using Xunit;

namespace SocietySaaS.UnitTests;

public class BillServiceTests
{
    private readonly Mock<IBillRepository> _billRepositoryMock;
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly Mock<IChargeRepository> _chargeRepositoryMock;
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly BillService _service;

    public BillServiceTests()
    {
        _billRepositoryMock = new Mock<IBillRepository>();
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _chargeRepositoryMock = new Mock<IChargeRepository>();
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

        _service = new BillService(
            _billRepositoryMock.Object,
            _flatRepositoryMock.Object,
            _chargeRepositoryMock.Object,
            _contextMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task GenerateAsync_WithValidFlat_CreatesBill()
    {
        var flatId = Guid.NewGuid();
        var tenantId = _currentUserMock.Object.TenantId!.Value;
        var flat = new Flat { Id = flatId, FlatNumber = "101", TenantId = tenantId };
        var charges = new List<Charge>
        {
            new() { Id = Guid.NewGuid(), Name = "Maintenance", Amount = 5000, IsRecurring = true, IsActive = true, TenantId = tenantId }
        };
        _flatRepositoryMock.Setup(r => r.GetByIdAsync(flatId)).ReturnsAsync(flat);
        _chargeRepositoryMock.Setup(r => r.GetActiveChargesAsync()).ReturnsAsync(charges);
        _billRepositoryMock.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(new List<Bill>());

        var billLines = new List<Application.Common.DTOs.BillLineRequest>
        {
            new(charges[0].Id, 5000, "Maintenance")
        };
        var result = await _service.GenerateAsync(new Application.Common.DTOs.GenerateBillRequest(
            "Sep-2026", DateTime.UtcNow.AddDays(30), flatId, billLines));

        Assert.NotNull(result);
        Assert.Equal(5000, result.GrandTotal);
        _billRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Bill>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_WithValidBill_SetsCancelled()
    {
        var bill = new Bill { Id = Guid.NewGuid(), Status = "Pending", IsCancelled = false };
        _billRepositoryMock.Setup(r => r.GetByIdAsync(bill.Id)).ReturnsAsync(bill);

        var result = await _service.CancelAsync(bill.Id, "Test cancellation");

        Assert.True(bill.IsCancelled);
        Assert.Equal("Cancelled", bill.Status);
    }
}
