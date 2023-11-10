using SendGrid;

namespace TFGames.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task<Response> SendEmail(string from, string to, string subject, string emailBody);
    }
}