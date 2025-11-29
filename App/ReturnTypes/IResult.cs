namespace TestShopApp.App.ReturnTypes
{
    public interface IResult
    {
        string Message { get; }
        ErrorType ErrorType { get; }
        bool IsSuccess { get; }
        bool IsFailure { get; }
    }

    public interface IResult<out T> : IResult
    {
        T Value { get; }
    }
}
