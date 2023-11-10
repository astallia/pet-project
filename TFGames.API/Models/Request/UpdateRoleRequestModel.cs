using System.Text.Json.Serialization;
using TFGames.Common.Enums;

namespace TFGames.API.Models.Request
{
    public class UpdateRoleRequestModel
    {
        public string UserId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoleNames Role { get; set; }
    }
}