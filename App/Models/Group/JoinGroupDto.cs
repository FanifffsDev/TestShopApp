using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestShopApp.App.Models.Group
{
    public class JoinGroupDto
    {
        [Required(ErrorMessage = "Invite code is crucial")]
        [JsonPropertyName("inviteCode")]
        public string InviteCode { get; set; }
    }
}
