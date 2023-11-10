using System.Net;
using NSubstitute;
using SendGrid;
using SendGrid.Helpers.Mail;
using TFGames.API.Services;
using TFGames.DAL.Entities;

namespace TFGames.Tests
{
    public class EmailServiceTest
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly Response okResponse = new Response(HttpStatusCode.OK, null, null);

        public EmailServiceTest()
        {
            _sendGridClient = Substitute.For<ISendGridClient>();
            _sendGridClient.SendEmailAsync(Arg.Any<SendGridMessage>()).Returns(okResponse);
        }

        [Fact]
        public async void SendEmail_ShouldReturnStatusOk()
        {
            // Arrange
            var fromEmail = "from@gmail.com";
            var toEmail = "to@gmail.com";
            var subject = "test";
            var emailBody = "test body";

            var user = new User { Email = toEmail };
           
            var emailService = new EmailService(_sendGridClient);

            // Action
            var actual = await emailService.SendEmail(fromEmail, toEmail, subject, emailBody);
            
            // Assert
            Assert.Equal(okResponse.StatusCode, actual.StatusCode);
            await _sendGridClient.Received(1).SendEmailAsync(Arg.Is<SendGridMessage>(x => x.From.Email == fromEmail));
        }
    }
}