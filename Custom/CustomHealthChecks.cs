using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck.Custom
{
    public class CustomHealthChecks : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var catUrl = "https://http.cat/401";

            var client = new HttpClient();

            client.BaseAddress = new Uri(catUrl);

            HttpResponseMessage response = await client.GetAsync("");

            return response.StatusCode == HttpStatusCode.OK ? 
                await Task.FromResult(new HealthCheckResult(
                      status: HealthStatus.Healthy,
                      description: "The API is healthy （。＾▽＾）")) :
                await Task.FromResult(new HealthCheckResult(
                      status: HealthStatus.Unhealthy, 
                      description: "The API is sick (‘﹏*๑)"));
        }
    }
}
