using Moq;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Services;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;
using Xunit;

namespace SocietySaaS.UnitTests;

public class ChargeServiceTests
{
    private readonly Mock<IChargeRepository> _chargeRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly ChargeService _service;

    public ChargeServiceTests()
    {
        _chargeRepositoryMock = new Mock<IChargeRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(Guid.NewGuid());
        _service = new ChargeService(_chargeRepositoryMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidCharge_ReturnsCharge()
    {
        _chargeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Charge>())).ReturnsAsync(new Charge());

        var result = await _service.CreateAsync(new Application.Common.DTOs.CreateChargeRequest(
            "Maintenance", "Monthly maintenance", "Fixed", 5000, true));

        Assert.NotNull(result);
        Assert.Equal("Maintenance", result.Name);
    }
}
