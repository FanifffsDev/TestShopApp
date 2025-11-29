using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestShopApp.App.Models.Group
{
    public class GroupMemberDto
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("photoUrl")]
        public string? PhotoUrl { get; set; }

        [JsonPropertyName("isHeadman")]
        public bool IsHeadman { get; set; }
    }
}
