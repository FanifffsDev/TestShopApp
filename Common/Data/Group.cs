using System.Text.Json.Serialization;

namespace TestShopApp.Common.Data;

public class Group
{
    [JsonPropertyName("ownerId")]
    public long OwnerId { get; set; }
    
    [JsonPropertyName("number")]
    public string Number { get; set; }
        
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("users")]
    public List<User> Users { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}