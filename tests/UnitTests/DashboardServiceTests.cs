using Moq;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Services;
using SocietySaaS.Domain.Interfaces;
using Xunit;

namespace SocietySaaS.UnitTests;

public class DashboardServiceTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly Mock<IMemberRepository> _memberRepositoryMock;
    private readonly Mock<IBillRepository> _billRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _memberRepositoryMock = new Mock<IMemberRepository>();
        _billRepositoryMock = new Mock<IBillRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(Guid.NewGuid());
        _service = new DashboardService(
            _flatRepositoryMock.Object,
            _memberRepositoryMock.Object,
            _billRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _tenantRepositoryMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsDashboard()
    {
        _flatRepositoryMock.Setup(r => r.GetAllWithMembersAsync()).ReturnsAsync(new List<Domain.Entities.Flat>());
        _memberRepositoryMock.Setup(r => r.GetAllWithFlatAsync()).ReturnsAsync(new List<Domain.Entities.Member>());
        _billRepositoryMock.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(new List<Domain.Entities.Bill>());
        _paymentRepositoryMock.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(new List<Domain.Entities.Payment>());

        var result = await _service.GetDashboardAsync();

        Assert.NotNull(result);
    }
}
