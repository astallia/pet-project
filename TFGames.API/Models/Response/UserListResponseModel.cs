namespace TFGames.API.Models.Response
{
    public class UserListResponseModel
    {
        public List<UserResponseModel> Users { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }
    }
}