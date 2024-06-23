namespace Comanda.WebApi.Utils;

public class HealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        bool healthCheckResultHealthy = true;

        if (healthCheckResultHealthy)
        {
            return Task.FromResult(HealthCheckResult.Healthy("O serviço está funcionando corretamente."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("O serviço está enfrentando problemas."));
    }
}