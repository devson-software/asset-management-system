using SendGrid;
using SendGrid.Helpers.Mail;

namespace AssetPro.Api.Infrastructure.Email;

public class SendGridSettings
{
    public const string SectionName = "SendGrid";

    public string ApiKey { get; set; } = default!;
    public string FromEmail { get; set; } = default!;
    public string FromName { get; set; } = "AssetPro";
}

public class SendGridEmailService : IEmailService
{
    private readonly SendGridSettings _settings;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(
        Microsoft.Extensions.Options.IOptions<SendGridSettings> settings,
        ILogger<SendGridEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string toName, string subject, string htmlContent, CancellationToken ct = default)
    {
        var client = new SendGridClient(_settings.ApiKey);
        var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
        var to = new EmailAddress(toEmail, toName);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent);

        var response = await client.SendEmailAsync(msg, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Body.ReadAsStringAsync(ct);
            _logger.LogError("SendGrid failed with {StatusCode}: {Body}", response.StatusCode, body);
        }
    }
}
