using System.Text.Json.Serialization;

namespace TestShopApp.Domain.Base;

public class User
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("tgFirstName")]
    public string? TgFirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("tgLastName")]
    public string? TgLastName { get; set; }

    [JsonPropertyName("thirdName")]
    public string? ThirdName { get; set; }
        
    [JsonPropertyName("groupNumber")]
    public string? GroupNumber { get; set; }

    [JsonIgnore]
    public Group? Group { get; set; }

    [JsonPropertyName("headmanOf")]
    public string? HeadmanOf { get; set; }
        
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
        
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("photoUrl")]
    public string? PhotoUrl { get; set; }

    [JsonPropertyName("languageCode")]
    public string? LanguageCode { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }
}