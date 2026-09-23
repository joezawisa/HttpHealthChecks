# HttpHealthChecks

A NuGet package providing quick and easy health checks for .NET applications that rely on external HTTP web APIs.

[![NuGet Badge](https://img.shields.io/nuget/v/HealthChecks.Http)](https://www.nuget.org/packages/HealthChecks.Http)
[![Release](https://github.com/joezawisa/HttpHealthChecks/actions/workflows/release.yml/badge.svg)][Release Pipeline]

## Getting Started

For guidance on health checks in general, refer to [Health checks in ASP.NET Core][Microsoft Health Check Documentation].

To register an HTTP health check, call `AddHttpHealthCheck` in your `Program.cs` (or similar startup code).

```csharp
using HealthChecks.Http;
using Microsoft.Builder;
using Microsoft.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder(args);
webAppBuilder.Services.AddHttpClient();

...

IHealthChecksBuilder healthChecksBuilder = webAppBuilder.Services.AddHealthChecks();
healthChecksBuilder.AddHttpHealthCheck(name: "Example", url: "https://example.com/health");
```

Be sure to also call `AddHttpClient` to make an `IHttpClientFactory` available to your health checks.

Multiple HTTP health checks can be registered with
different names.

```csharp
healthChecksBuilder.AddHttpHealthCheck(name: "AnotherExample", url: "https://example2.com/health");
```

These examples both use `/health` endpoints, demonstrating how you can consume health checks from another website in your own health checks. However, this is not required; you can use any URL you choose. Any 200-series response codes will be considered healthy, and any other response will be considered unhealthy.

To use an HTTP health check once it is registered, you can call `UseHealthChecks` or `MapHealthChecks` as with any other health check. [Health checks in ASP.NET Core][Microsoft Health Check Documentation] explains when it's appropriate to use each one.

```csharp
...
WebApplication app = builder.Build();
...

app.UseHealthChecks(path: "/health");
// OR
app.MapHealthChecks(pattern: "/health");

...
app.Run();
```

## Feedback

Notice something wrong? Create an issue in [joezawisa/HttpHealthChecks][GitHub Repository] on GitHub.

Have an idea to make this better? Start a discussion in [joezawisa/HttpHealthChecks][GitHub Repository] on GitHub.

## Contributing

Submit a pull request to [joezawisa/HttpHealthChecks][GitHub Repository] on GitHub. If you are adding a new feature, please create an issue to discuss it first. If you are fixing a bug, feel free to skip right to a pull request.

This project uses [GitVersion](https://gitversion.net) for [semantic versioning](https://semver.org). The package version will not be incremented by default; you must explicitly include `+semver: major|minor|patch` as a commit message footer. When the package version _is_ incremented, the [release pipeline][Release Pipeline] will publish it automatically.

## License

This project is licensed under the terms of the MIT license.

[GitHub Repository]: https://github.com/joezawisa/HttpHealthChecks
[Release Pipeline]: https://github.com/joezawisa/HttpHealthChecks/actions/workflows/release.yml
[Microsoft Health Check Documentation]: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks