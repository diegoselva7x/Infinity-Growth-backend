using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;

namespace AppLogic.Services
{
    public interface IEmailService
    {
        Task<string> SendEmail(string emailAddres, string subject, string plainTextContent, string htmlContent);
    }
    public class Email_Service : IEmailService
    {
        private readonly string _connectionString;
        private readonly string _senderAddress;

        public Email_Service(IConfiguration configuration)
        {
            _connectionString = configuration["Azure:ConnectionStrings:DefaultConnectionEmail"];
            _senderAddress = configuration["Azure:ConnectionStrings:SenderAddressEmail"];
        }

        public async Task<string> SendEmail(string emailAddres, string subject, string plainTextContent, string htmlContent)
        {
            var emailClient = new EmailClient(_connectionString);
            var emailMessage = new EmailMessage(
                senderAddress: _senderAddress,
                content: new EmailContent(subject)
                {
                    PlainText = plainTextContent,
                    Html = htmlContent
                },
                recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(emailAddres) })
            );

            EmailSendOperation emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);
            return emailSendOperation.Value.Status.ToString();
        }
    }
}
