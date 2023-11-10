namespace TFGames.Common.Constants
{
    public class ErrorMessages
    {
        public const string BadRequest = "The server cannot process the request. Check the entered data.";

        public const string Conflict = "The request could not be processed because of the conflict.";

        public const string NotFound = "The server cannot find the requested resource.";

        public const string InternalServerError = "500 Internal Server Error. Sorry something went wrong.";

        public const string UnauthorizedAccess = "The request was not completed. There are no valid credentials to authenticate the requested resource.";

        public const string UnexpectedError = "Unexpected error has occured. Try again.";

        public const string InvalidPassword = "Invalid email/username or password. Try again.";

        public const string UserExist = "User already exists.";

        public const string DifferentPassword = "Password and Repeat Password are not equal.";

        public const string WrongLoginFormat = "Wrong email or username format. Try again.";

        public const string InvalidRequest = "Invalid client request.";

        public const string InvalidTokens = "Invalid access token or refresh token.";

        public const string TokenExpired = "Refresh token expired.";

        public const string InvalidTokensOrDate = "Invalid refresh token or token expired.";

        public const string PasswordLength = "The Password field must be at least 8 characters long";

        public const string PasswordContainOnlyLatin = "Password should contain only latin letters. {0}";

        public const string InappropriateNameFormat = "Only letters. Starts with uppercase.";

        public const string InappropriateEmailFormat = "Should match email format.";

        public const string IncorrectPassword = "Password should contain at least 8 characters. One uppercase, one lowercase, one number, and one special character (!%*?&).";

        public const string RoleAlreadyAssigned = "User already has such role.";

        public const string UsernameLength = "The Username field must be at least 3 characters long";

        public const string WrongFileSize = "Wrong file size. The maximum size of image is 5 mb. Try again";

        public const string WrongTagsQuantity = "Wrong tags quantity. The maximum number of tags is five. Try again";

        public const string WrongTagsName = "Wrong tag name. Try again";

        public const string WrongYear = "Wrong length of year. Try again";

        public const string WrongFormat = "Wrong file format. Available extension is jpeg, jpe, jpg, jif, jfif, png. Try again";

        public const string NoPreviewArrticle = "The server cannot process the request. Lack of previews for articles in the database.";

        public const string ForbiddenToEditComment = "Forrbiden to edit comment";

        public const string ForbiddenToEdit = "Forbidden to edit.";

        public const string Forbidden = "Forrbiden.";

        public const string NoResult = "No results.";

        public const string NotFoundImage = "The server cannot find the image.";

        public const string NotConfirmedEmail = "The email is not confirmed.";

        public const string RefreshTokenHasBeenUsed = "This link has been already used, please request another one.";

        public const string CommentWhiteSpace = "Comment cannot consist only of spaces.";
    }
}
