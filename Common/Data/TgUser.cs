using System.Text.Json.Serialization;

namespace TestShopApp.Common.Data
{
    public class TgUser
    {
        [JsonPropertyName("user")]
        public long Id { get; set; }
        
        [JsonPropertyName("is_premium")]
        public bool IsPremium { get; set; }
        
        [JsonPropertyName("allows_write_to_pm")]
        public bool AllowsWriteToPm { get; set; }
        
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }
        
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }
        
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        
        [JsonPropertyName("photo_url")]
        public string? PhotoUrl { get; set; }
        
        [JsonPropertyName("language_code")]
        public string? LanguageCode { get; set; }
    }
}
