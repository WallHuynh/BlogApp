using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BlogApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private readonly IConfiguration Configuration;

        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            string sendGridKey = Environment.ExpandEnvironmentVariables("sendGridKey");

            if (string.IsNullOrEmpty(sendGridKey))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(sendGridKey, subject, message, toEmail);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            string senderEmail = Environment.ExpandEnvironmentVariables("senderEmail");
            string senderName = Environment.ExpandEnvironmentVariables("senderName");

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(senderEmail, senderName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(
                response.IsSuccessStatusCode
                    ? $"Email to {toEmail} queued successfully!"
                    : $"Failure Email to {toEmail}"
            );
        }
    }
}
