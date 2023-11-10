namespace TFGames.Common.Constants
{
    public class Mail
    {
        public const string FromName = "TechFabric";

        public const string FromEmail = "konstantin.donchenko@techfabric.com";

        public const string ConfirmationSubject = "TFGames: Email Confirmation";

        public const string ResetPasswordSubject = "TFGames: Reset Password";

        public const string ConfirmationTextHtml = "<p>Hello {0},</p><p> Welcome to our <u>Games</u> team! You have successfully registered on our site, the last thing to do is to confirm your email, so you will be able to use our site as a user. </p><a href='{1}/users/confirm/{2}?token={3}'>Click to confirm email.</a><p>If you did not expect to receive this message, please ignore. </p> <p>TechFabric Games.</p>";

        public const string ResetPasswordTextHtml = "<p>Hello {0},</p><p>We received a request to reset the password associated with your account. To proceed with the password reset, please click on the following link:</p><a href='{1}/reset-password/{2}?token={3}'>Click to reset password.</a><p>If you did not request this password reset or believe you received this message in error, please ignore it. Your account security is important to us.</p><p>TechFabric Games</p>";
    }
}