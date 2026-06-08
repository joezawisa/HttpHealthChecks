using HealthChecks.Http;

WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
webAppBuilder.Services.AddHttpClient();

// Add health checks.
IHealthChecksBuilder healthChecksBuilder = webAppBuilder.Services.AddHealthChecks();
healthChecksBuilder.AddHttpHealthCheck(name: "200Ok", url: "http://localhost:5000/ok");
healthChecksBuilder.AddHttpHealthCheck(name: "400BadRequest", url: "http://localhost:5000/bad-request");
healthChecksBuilder.AddHttpHealthCheck(name: "500InternalServerError", url: "http://localhost:5000/internal-server-error");
healthChecksBuilder.AddHttpHealthCheck(name: "Unreachable", url: "http://localhost:80");

WebApplication app = webAppBuilder.Build();

// Add endpoints that are guaranteed to always return the same response code for
// the health checks to use. Realistically, health checks would not refer to
// endpoints on the same server, but this works for testing purposes.
app.MapGet("/ok", () => Results.Ok());
app.MapGet("/bad-request", () => Results.BadRequest());
app.MapGet("/internal-server-error", () => Results.InternalServerError());

// Enable health checks.
// This first one combines the three others and should report unhealthy.
app.MapHealthChecks(
    pattern: "/health",
    options: new() { Predicate = _ => true }
);
// This one will receive a 200 OK and should report healthy.
app.MapHealthChecks(
    pattern: "/health/ok",
    options: new() { Predicate = hc => hc.Name == "200Ok" }
);
// This one will receive a 400 Bad Request and should report unhealthy.
app.MapHealthChecks(
    pattern: "/health/bad-request",
    options: new() { Predicate = hc => hc.Name == "400BadRequest" }
);
// This one will receive a 500 Internal Server Error and should report unhealthy.
app.MapHealthChecks(
    pattern: "/health/internal-server-error",
    options: new() { Predicate = hc => hc.Name == "500InternalServerError" }
);
// This one will not receive a response and should report unhealthy.
app.MapHealthChecks(
    pattern: "/health/unreachable",
    options: new() { Predicate = hc => hc.Name == "Unreachable" }
);

app.Run();