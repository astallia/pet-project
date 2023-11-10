using SendGrid;
using SendGrid.Helpers.Mail;
using TFGames.API.Services.Interfaces;

namespace TFGames.API.Services
{
    public class EmailService : IEmailService
    {        
        private readonly ISendGridClient _sendGridClient;

        public EmailService(ISendGridClient sendGridClient)
        {
            _sendGridClient = sendGridClient;
        }

        public async Task<Response> SendEmail(string from, string to, string subject, string emailBody)
        {
            var message = new SendGridMessage()
            {
                From = new EmailAddress(from),
                Subject = subject,
                HtmlContent = emailBody
            };

            message.AddTo(new EmailAddress(to));
            message.SetClickTracking(false, false);

            return await _sendGridClient.SendEmailAsync(message);
        }
    }
}