namespace TestShopApp.App.ReturnTypes
{
    public static class ResultImport
    {
        public static Result Ok() => Result.Ok();
        public static Result Fail(ErrorType errorType, string message) => Result.Fail(errorType, message);
        public static Result NotFound(string message) => Result.NotFound(message);
        public static Result InvalidArgument(string message) => Result.InvalidArgument(message);
        public static Result AlreadyExists(string message) => Result.AlreadyExists(message);
        public static Result Unauthorized(string message) => Result.Unauthorized(message);
        public static Result Conflict(string message) => Result.Conflict(message);
        public static Result Forbidden(string message) => Result.Forbidden(message);
        public static Result Internal(string message) => Result.Internal(message);

        public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);
        public static Result<T> Fail<T>(ErrorType errorType, string message) => Result<T>.Fail(errorType, message);
        public static Result<T> NotFound<T>(string message) => Result<T>.NotFound(message);
        public static Result<T> InvalidArgument<T>(string message) => Result<T>.InvalidArgument(message);
        public static Result<T> AlreadyExists<T>(string message) => Result<T>.AlreadyExists(message);
        public static Result<T> Unauthorized<T>(string message) => Result<T>.Unauthorized(message);
        public static Result<T> Conflict<T>(string message) => Result<T>.Conflict(message);
        public static Result<T> Forbidden<T>(string message) => Result<T>.Forbidden(message);
        public static Result<T> Internal<T>(string message) => Result<T>.Internal(message);
    }
}
