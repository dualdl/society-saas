using Moq;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Services;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Domain.Interfaces;
using Xunit;

namespace SocietySaaS.UnitTests;

public class FlatServiceTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly FlatService _service;

    public FlatServiceTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.TenantId).Returns(Guid.NewGuid());
        _service = new FlatService(_flatRepositoryMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedFlats()
    {
        var tenantId = _currentUserMock.Object.TenantId!.Value;
        var flats = new List<Flat>
        {
            new() { Id = Guid.NewGuid(), FlatNumber = "101", TenantId = tenantId, Wing = new Wing { Name = "A" }, Members = new List<Member>() }
        };
        _flatRepositoryMock.Setup(r => r.GetAllWithMembersAsync()).ReturnsAsync(flats);

        var result = await _service.GetAllAsync(1, 10, null);

        Assert.NotNull(result);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsFlat()
    {
        var flat = new Flat { Id = Guid.NewGuid(), FlatNumber = "101", Wing = new Wing { Name = "A" }, Members = new List<Member>() };
        _flatRepositoryMock.Setup(r => r.GetWithDetailsAsync(flat.Id)).ReturnsAsync(flat);

        var result = await _service.GetByIdAsync(flat.Id);

        Assert.NotNull(result);
        Assert.Equal("101", result.FlatNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        _flatRepositoryMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>())).ReturnsAsync((Flat?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_SetsIsDeleted()
    {
        var flat = new Flat { Id = Guid.NewGuid(), IsDeleted = false };
        _flatRepositoryMock.Setup(r => r.GetByIdAsync(flat.Id)).ReturnsAsync(flat);

        await _service.DeleteAsync(flat.Id);

        Assert.True(flat.IsDeleted);
        _flatRepositoryMock.Verify(r => r.UpdateAsync(flat), Times.Once);
    }
}
