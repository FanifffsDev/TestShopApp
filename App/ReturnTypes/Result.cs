using System.Text.Json.Serialization;

namespace TestShopApp.App.ReturnTypes
{
    public class Result : IResult, Microsoft.AspNetCore.Http.IResult
    {
        public string Message { get; } = string.Empty;
        public ErrorType ErrorType { get; }
        public bool IsSuccess => ErrorType == ErrorType.None;
        public bool IsFailure => !IsSuccess;

        protected Dictionary<string, string> Headers { get; } = new();
        protected Dictionary<string, object> Fields { get; } = new();


        protected Result(ErrorType errorType) => ErrorType = errorType;

        protected Result(ErrorType errorType, string message = "")
        {
            Message = message;
            ErrorType = errorType;
        }

        public Result WithHeader(string name, string value)
        {
            Headers[name] = value;
            return this;
        }

        public Result WithField(string name, object value)
        {
            Fields[name] = value;
            return this;
        }

        public Result WithFieldIf(string name, object value, bool condition)
        {
            if (condition)
                Fields[name] = value;

            return this;
        }

        public static Result Ok() => new(ErrorType.None);
        public static Result Fail(ErrorType errorType, string message) => new(errorType, message);
        public static Result NotFound(string message) => new(ErrorType.NotFound, message);
        public static Result InvalidArgument(string message) => new(ErrorType.InvalidArgument, message);
        public static Result AlreadyExists(string message) => new(ErrorType.AlreadyExists, message);
        public static Result Unauthorized(string message) => new(ErrorType.Unauthorized, message);
        public static Result Conflict(string message) => new(ErrorType.Conflict, message);
        public static Result Forbidden(string message) => new(ErrorType.Conflict, message);
        public static Result Internal(string message) => new(ErrorType.Internal, message);

        public int GetStatusCode() => ErrorType switch
        {
            ErrorType.None => StatusCodes.Status200OK,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.InvalidArgument => StatusCodes.Status400BadRequest,
            ErrorType.AlreadyExists => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Internal => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        public virtual async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = GetStatusCode();

            foreach (var header in Headers)
            {
                httpContext.Response.Headers[header.Key] = header.Value;
            }

            if (IsSuccess)
            {
                var response = new Dictionary<string, object>
                {
                    ["success"] = true
                };

                foreach (var field in Fields)
                {
                    response[field.Key] = field.Value;
                }

                await httpContext.Response.WriteAsJsonAsync(response);
            }
            else
            {
                await httpContext.Response.WriteAsJsonAsync(new { success = false, error = Message });
            }
        }
    }

    public class Result<T> : Result, IResult<T>, Microsoft.AspNetCore.Http.IResult
    {
        public T Value { get; } = default!;

        private Result(T value) : base(ErrorType.None)
        {
            Value = value;
        }

        private Result(ErrorType errorType, string message) : base(errorType, message)
        {
            Value = default!;
        }


        public new Result<T> WithHeader(string name, string value)
        {
            Headers[name] = value;
            return this;
        }

        public new Result<T> WithField(string name, object value)
        {
            Fields[name] = value;
            return this;
        }

        public new Result<T> WithFieldIf(string name, object value, bool condition)
        {
            if (condition)
                Fields[name] = value;

            return this;
        }

        public static Result<T> Ok(T value) => new(value);
        public static new Result<T> Fail(ErrorType errorType, string message) => new(errorType, message);
        public static new Result<T> NotFound(string message) => new(ErrorType.NotFound, message);
        public static new Result<T> InvalidArgument(string message) => new(ErrorType.InvalidArgument, message);
        public static new Result<T> AlreadyExists(string message) => new(ErrorType.AlreadyExists, message);
        public static new Result<T> Unauthorized(string message) => new(ErrorType.Unauthorized, message);
        public static new Result<T> Conflict(string message) => new(ErrorType.Conflict, message);
        public static new Result<T> Forbidden(string message) => new(ErrorType.Conflict, message);
        public static new Result<T> Internal(string message) => new(ErrorType.Internal, message);

        public override async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = GetStatusCode();

            foreach (var header in Headers)
            {
                httpContext.Response.Headers[header.Key] = header.Value;
            }

            if (IsSuccess)
            {
                var response = new Dictionary<string, object>
                {
                    ["success"] = true,
                    //["data"] = Value!
                };

                foreach (var field in Fields)
                {
                    response[field.Key] = field.Value;
                }

                await httpContext.Response.WriteAsJsonAsync(response);
            }
            else
            {
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = Message
                });
            }
        }
    }

    public enum ErrorType
    {
        None,
        NotFound,
        InvalidArgument,
        AlreadyExists,
        Unauthorized,
        Conflict,
        Forbidden,
        Internal
    }
}