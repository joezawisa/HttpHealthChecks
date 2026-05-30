using System.Net;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Http.UnitTests;

file class MockHttpMessageHandler(Func<HttpResponseMessage> responseGenerator) : HttpMessageHandler
{
    protected override HttpResponseMessage Send(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    ) => responseGenerator();

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    ) => Task.FromResult(result: responseGenerator());
}

file class MockHttpClientFactory(
    Func<HttpResponseMessage> responseGenerator
) : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return new HttpClient(handler: new MockHttpMessageHandler(responseGenerator: responseGenerator));
    }
}

public class HttpHealthCheckTests(ITestContextAccessor testContextAccessor)
{
    [Fact]
    public async Task CheckHealthAsync_GivenSuccessResponse_ReturnsHealthyStatus()
    {
        // Arrange
        HttpResponseMessage mockResponse = new(statusCode: HttpStatusCode.OK);
        MockHttpClientFactory mockHttpClientFactory = new(responseGenerator: () => mockResponse);
        HttpHealthCheck healthCheck = new(httpClientFactory: mockHttpClientFactory, url: "https://example.com");
        HealthCheckContext healthCheckContext = new() { Registration = new(
            name: "test",
            instance: healthCheck,
            failureStatus: null,
            tags: []
        ) };

        // Act
        HealthCheckResult result = await healthCheck.CheckHealthAsync(
            context: healthCheckContext,
            cancellationToken: testContextAccessor.Current.CancellationToken
        );

        // Assert
        Assert.Equal(expected: HealthStatus.Healthy, actual: result.Status);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, HealthStatus.Unhealthy)]
    [InlineData(HttpStatusCode.BadRequest, HealthStatus.Degraded)]
    [InlineData(HttpStatusCode.InternalServerError, HealthStatus.Unhealthy)]
    [InlineData(HttpStatusCode.InternalServerError, HealthStatus.Degraded)]
    public async Task CheckHealthAsync_GivenFailureResponse_ReturnsFailureStatus(HttpStatusCode statusCode, HealthStatus failureStatus)
    {
        // Arrange
        HttpResponseMessage mockResponse = new(statusCode: statusCode);
        MockHttpClientFactory mockHttpClientFactory = new(responseGenerator: () => mockResponse);
        HttpHealthCheck healthCheck = new(httpClientFactory: mockHttpClientFactory, url: "https://example.com");
        HealthCheckContext healthCheckContext = new() { Registration = new(
            name: "test",
            instance: healthCheck,
            failureStatus: failureStatus,
            tags: []
        ) };

        // Act
        HealthCheckResult result = await healthCheck.CheckHealthAsync(
            context: healthCheckContext,
            cancellationToken: testContextAccessor.Current.CancellationToken
        );

        // Assert
        Assert.Equal(expected: failureStatus, actual: result.Status);
    }

    [Theory]
    [InlineData(HealthStatus.Unhealthy)]
    [InlineData(HealthStatus.Degraded)]
    public async Task CheckHealthAsync_GivenNoResponse_ReturnsFailureStatus(HealthStatus failureStatus)
    {
        // Arrange
        HttpRequestException exception = new();
        MockHttpClientFactory mockHttpClientFactory = new(responseGenerator: () => throw exception);
        HttpHealthCheck healthCheck = new(httpClientFactory: mockHttpClientFactory, url: "https://example.com");
        HealthCheckContext healthCheckContext = new() { Registration = new(
            name: "test",
            instance: healthCheck,
            failureStatus: failureStatus,
            tags: []
        ) };

        // Act
        HealthCheckResult result = await healthCheck.CheckHealthAsync(
            context: healthCheckContext,
            cancellationToken: testContextAccessor.Current.CancellationToken
        );

        // Assert
        Assert.Equal(expected: failureStatus, actual: result.Status);
        Assert.Equal(expected: exception, actual: result.Exception);
    }
}