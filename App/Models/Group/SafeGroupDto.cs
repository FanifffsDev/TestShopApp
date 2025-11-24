using System.Text.Json.Serialization;

namespace TestShopApp.App.Models.Group;

public class SafeGroupDto
{
    [JsonPropertyName("number")]
    public string Number { get; set; }
        
    [JsonPropertyName("name")]
    public string Name { get; set; }
}