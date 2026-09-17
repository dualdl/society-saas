using System.Net;
using Xunit;

namespace SocietySaaS.IntegrationTests;

public class SwaggerTests : IClassFixture<WebApplicationFactory>
{
    private readonly HttpClient _client;

    public SwaggerTests(WebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSwagger_ReturnsOk()
    {
        var response = await _client.GetAsync("/swagger");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
