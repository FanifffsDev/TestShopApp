using System.Text.Json.Serialization;

namespace TestShopApp.Common.Data;

public class User
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
        
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }
        
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }
    
    [JsonPropertyName("thirdName")]
    public string? ThirdName { get; set; }
        
    [JsonPropertyName("group")]
    public string? Group { get; set; }
    
    [JsonPropertyName("headmanOf")]
    public string? HeadmenOf { get; set; }
        
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
        
    [JsonPropertyName("role")]
    public string Role { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}