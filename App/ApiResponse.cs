using System.Text.Json.Serialization;

namespace TestShopApp.App;

public class ApiResponse
{
    public bool Success { get; private set; }

    private readonly Dictionary<string, object> _fields = [];

    [JsonExtensionData] public IDictionary<string, object> Fields => _fields;

    public static ApiResponse Ok() => new ApiResponse { Success = true };
    public static ApiResponse Fail() => new ApiResponse { Success = false };
        
    public ApiResponse WithField(string name, object value)
    {
        _fields[name] = value;
        return this;
    }
}