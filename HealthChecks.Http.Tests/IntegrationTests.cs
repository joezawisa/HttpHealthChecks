using System.Net;

namespace HealthChecks.Http.Tests;

[Trait("Category", "Integration")]
public class IntegrationTests {
    private readonly HttpClient _httpClient = new() { BaseAddress = new("http://localhost:5000") };

    [Fact]
    public async Task HealthCheck_GivenSuccessResponse_ReturnsOk()
    {
        // Act
        HttpResponseMessage result = await _httpClient.GetAsync(
            requestUri: "/health/ok",
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: result.StatusCode);
    }

    [Theory]
    [InlineData("/health")]
    [InlineData("/health/bad-request")]
    [InlineData("/health/internal-server-error")]
    [InlineData("/health/unreachable")]
    public async Task HealthCheck_GivenFailureResponse_ReturnsUnavailable(string path)
    {
        // Act
        HttpResponseMessage result = await _httpClient.GetAsync(
            requestUri: path,
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(expected: HttpStatusCode.ServiceUnavailable, actual: result.StatusCode);
    }
}