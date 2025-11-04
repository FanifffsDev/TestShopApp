namespace TestShopApp.Common.Data
{
    public record ExecutionResult(bool success, string message = "");
    public record ExecutionResult<T>(bool success, T Value, string message = "");
}
