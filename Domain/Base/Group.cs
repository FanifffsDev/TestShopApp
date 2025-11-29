using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestShopApp.Domain.Base;

public class Group
{
    [Key]
    [JsonPropertyName("number")]
    public string Number { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore]
    public ICollection<User> Students { get; set; } = new List<User>();

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }
}