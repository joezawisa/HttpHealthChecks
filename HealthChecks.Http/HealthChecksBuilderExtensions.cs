using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Http
{
    /// <summary>
    /// Provides basic extension methods for registering <see cref="HttpHealthCheck"/>s in an <see cref="IHealthChecksBuilder"/>.
    /// </summary>
    public static class HealthChecksBuilderExtensions
    {
        /// <summary>
        /// Adds a new HTTP health check with the specified name and URL.
        /// <para/>
        /// <example>
        /// This shows how to add an HTTP health check with no custom options.
        /// <code>
        /// healthChecksBuilder.AddHttpHealthCheck(name: "Example", url: "https://example.com/health");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="builder"><inheritdoc cref="HealthChecksBuilderAddCheckExtensions.AddTypeActivatedCheck(IHealthChecksBuilder, string, object[])" /></param>
        /// <param name="name"><inheritdoc cref="HealthChecksBuilderAddCheckExtensions.AddTypeActivatedCheck(IHealthChecksBuilder, string, object[])" /></param>
        /// <param name="url">URL for the health check to request.</param>
        /// <inheritdoc cref="HealthChecksBuilderAddCheckExtensions.AddTypeActivatedCheck(IHealthChecksBuilder, string, object[])" path="/returns" />
        public static IHealthChecksBuilder AddHttpHealthCheck(
            this IHealthChecksBuilder builder,
            string name,
            string url
        ) => builder.AddTypeActivatedCheck<HttpHealthCheck>(
            name: name,
            args: url
        );

        /// <inheritdoc cref="AddHttpHealthCheck(IHealthChecksBuilder, string, string)"/>
        /// <inheritdoc cref="HealthChecksBuilderAddCheckExtensions.AddTypeActivatedCheck(IHealthChecksBuilder, string, HealthStatus?, object[])" path="/param"/>
        public static IHealthChecksBuilder AddHttpHealthCheck(
            this IHealthChecksBuilder builder,
            string name,
            string url,
            HealthStatus? failureStatus
        ) => builder.AddTypeActivatedCheck<HttpHealthCheck>(
            name: name,
            failureStatus: failureStatus,
            args: url
        );

        /// <inheritdoc cref="AddHttpHealthCheck(IHealthChecksBuilder, string, string, HealthStatus?)"/>
        /// <inheritdoc cref="HealthChecksBuilderAddCheckExtensions.AddTypeActivatedCheck(IHealthChecksBuilder, string, HealthStatus?, IEnumerable&lt;string&gt;, object[])" path="/param"/>
        public static IHealthChecksBuilder AddHttpHealthCheck(
            this IHealthChecksBuilder builder,
            string name,
            string url,
            HealthStatus? failureStatus,
            IEnumerable<string> tags
        ) => builder.AddTypeActivatedCheck<HttpHealthCheck>(
            name: name,
            failureStatus: failureStatus,
            tags: tags,
            args: url
        );

        /// <inheritdoc cref="AddHttpHealthCheck(IHealthChecksBuilder, string, string, HealthStatus?, IEnumerable&lt;string&gt;)"/>
        /// <inheritdoc cref="HealthChecksBuilderAddCheckExtensions.AddTypeActivatedCheck(IHealthChecksBuilder, string, HealthStatus?, IEnumerable&lt;string&gt;, TimeSpan, object[])" path="/param"/>
        public static IHealthChecksBuilder AddHttpHealthCheck(
            this IHealthChecksBuilder builder,
            string name,
            string url,
            HealthStatus? failureStatus,
            IEnumerable<string> tags,
            TimeSpan timeout
        ) => builder.AddTypeActivatedCheck<HttpHealthCheck>(
            name: name,
            failureStatus: failureStatus,
            tags: tags,
            timeout: timeout,
            args: url
        );
    }
}