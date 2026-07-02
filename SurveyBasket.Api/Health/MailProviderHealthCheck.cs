using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SurveyBasket.Api.Settings;

namespace SurveyBasket.Api.Health;

public class MailProviderHealthCheck(IOptions<MailSettings> mailSettings) : IHealthCheck
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Connection >> Smpt here from MailKit package
            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls, cancellationToken);
            await smtpClient.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password, cancellationToken);

            return await Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception e)
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy(exception: e));
        }
    }
}
