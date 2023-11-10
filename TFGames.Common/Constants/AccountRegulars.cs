namespace TFGames.Common.Constants
{
    public class AccountRegulars
    {
        public const string Name = @"^(?=.{2,50}$)([A-Z][a-z]+)$";

        public const string Password = @"^(?=.{8,50}$)((?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+)$";

        public const string EmailDomain = @"^(?=.{0,50}$)([A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]+)$";

        public const string Username = @"^(?=.{2,50}$)[A-Za-z0-9\-_+=!]*$";
    }
}