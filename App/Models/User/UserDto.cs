using System.Text.Json.Serialization;

namespace TestShopApp.App.Models.User
{
    public class UserDto
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string FirstName { get; set; }
        public string? TgFirstName { get; set; }
        public string LastName { get; set; }
        public string? TgLastName { get; set; }
        public string? ThirdName { get; set; }
        public string? GroupNumber { get; set; }
        public string? HeadmanOf { get; set; }
        public string? Subject { get; set; }
        public string Role { get; set; }
        public string? PhotoUrl { get; set; }
        public string? LanguageCode { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
