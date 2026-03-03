namespace AssetPro.Api.Infrastructure.Email;

public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlContent, CancellationToken ct = default);
}
