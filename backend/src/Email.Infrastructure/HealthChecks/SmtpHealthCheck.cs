using Email.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Email.Infrastructure.HealthChecks;

public sealed class SmtpHealthCheck : IHealthCheck
{
    private readonly SmtpOptions _options;

    public SmtpHealthCheck(IOptions<SmtpOptions> options)
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
            
            using var client = new SmtpClient();
            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                _options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto,
                cancellationToken);
            
            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
            }
            
            await client.DisconnectAsync(true, cancellationToken);
            
            var elapsed = DateTime.UtcNow - startTime;
            
            return HealthCheckResult.Healthy(
                "SMTP está disponível",
                data: new Dictionary<string, object>
                {
                    { "responseTime", $"{elapsed.TotalMilliseconds:F2}ms" },
                    { "host", _options.Host },
                    { "port", _options.Port }
                });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "SMTP não está disponível",
                ex,
                data: new Dictionary<string, object>
                {
                    { "error", ex.Message },
                    { "host", _options.Host },
                    { "port", _options.Port }
                });
        }
    }
}

