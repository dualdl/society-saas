using System.Net;
using Xunit;

namespace SocietySaaS.IntegrationTests;

public class HealthApiTests : IClassFixture<WebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthApiTests(WebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
