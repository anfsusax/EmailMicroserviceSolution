using Email.Infrastructure.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Email.Infrastructure.HealthChecks;

public sealed class RabbitMqHealthCheck : IHealthCheck
{
    private readonly RabbitMqOptions _options;

    public RabbitMqHealthCheck(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var startTime = DateTime.UtcNow;
            
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            
            var elapsed = DateTime.UtcNow - startTime;
            
            return HealthCheckResult.Healthy(
                "RabbitMQ está conectado",
                data: new Dictionary<string, object>
                {
                    { "responseTime", $"{elapsed.TotalMilliseconds:F2}ms" },
                    { "host", _options.HostName },
                    { "port", _options.Port }
                });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "RabbitMQ não está disponível",
                ex,
                data: new Dictionary<string, object>
                {
                    { "error", ex.Message },
                    { "host", _options.HostName },
                    { "port", _options.Port }
                });
        }
    }
}

