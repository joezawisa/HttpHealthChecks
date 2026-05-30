using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Http
{
    /// <summary>
    /// A health check that makes a request to an HTTP endpoint.
    /// </summary>
    public class HttpHealthCheck : IHealthCheck {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        /// <summary>
        /// Creates an <see cref="HttpHealthCheck"/>. Intended for dependency
        /// injection.
        /// </summary>
        /// <param name="httpClientFactory">Factory to create an HTTP client.</param>
        /// <param name="url">Full URL to check.</param>
        public HttpHealthCheck(
            IHttpClientFactory httpClientFactory,
            string url
        )
        {
            if (httpClientFactory is null) throw new ArgumentNullException(paramName: nameof(httpClientFactory));
            if (url is null) throw new ArgumentNullException(paramName: nameof(url));

            _httpClient = httpClientFactory.CreateClient();
            _url = url;
        }

        /// <inheritdoc/>
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        ) {
            if (context is null) throw new ArgumentNullException(paramName: nameof(context));

            try {
                HttpResponseMessage response = await _httpClient.GetAsync(
                    requestUri: _url,
                    completionOption: HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken: cancellationToken
                );

                ReadOnlyDictionary<string, object> data = new ReadOnlyDictionary<string, object>(
                    dictionary: new Dictionary<string, object>()
                    {
                        { nameof(response.StatusCode), response.StatusCode },
                        { nameof(response.Content), response.Content },
                        { nameof(response.Headers), response.Headers }
                    }
                );

                if (response.IsSuccessStatusCode) {
                    return HealthCheckResult.Healthy(
                        description: $"Received a success response from {_url}.",
                        data: data
                    );
                }
                
                return new HealthCheckResult(
                    status: context.Registration.FailureStatus,
                    description: $"Received a failure response from {_url}.",
                    data: data
                );
            } catch (HttpRequestException exception) {
                return new HealthCheckResult(
                    status: context.Registration.FailureStatus,
                    description: $"Did not receive a response from {_url}.",
                    exception: exception
                );
            }
        }
    }
}