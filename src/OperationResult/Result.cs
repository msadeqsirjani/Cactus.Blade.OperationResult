using System;
using System.Threading.Tasks;

namespace OperationResult
{
    public readonly struct Result<T>
    {
        public T Value { get; }
        public ActionResult ActionResult { get; }
        public string Message { get; }
        public Exception Exception { get; }
        public bool IsSuccess { get; }

        public Result(T value)
        {
            Value = value;
            IsSuccess = value is not System.Exception;
            Exception = null;
            Message = string.Empty;
            ActionResult = ActionResult.Success;
        }

        public Result(string message)
        {
            Value = default;
            IsSuccess = false;
            Exception = null;
            Message = message;
            ActionResult = ActionResult.Message;
        }

        public Result(Exception exception)
        {
            Value = default;
            IsSuccess = false;
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            Message = exception.Message;
            ActionResult = ActionResult.Exception;
        }

        public static implicit operator Result<T>(T value) => new(value);

        public static implicit operator Result<T>(string value) => new(value);

        public static implicit operator Result<T>(Exception exception) => new(exception);

        public Result<TEnd> ChangeInAnotherResult<TEnd>(Func<T, TEnd> converter)
            => IsSuccess
                ? new Result<TEnd>(converter(Value))
                : new Result<TEnd>(Exception);

        public Result ChangeInNoResult() =>
            IsSuccess
                ? new Result(ActionResult.Success)
                : new Result(Exception);

        public void Deconstruct(out ActionResult actionResult, out T value)
            => (actionResult, value) = (ActionResult, Value);

        public void Deconstruct(out ActionResult actionResult, out T value, out Exception exception)
            => (actionResult, value, exception) = (ActionResult, Value, Exception);

        public static implicit operator bool(Result<T> result)
            => result.IsSuccess;

        public static implicit operator Task<Result<T>>(Result<T> result)
            => result.AsTask;

        public Task<Result<T>> AsTask => Task.FromResult(this);
    }

    public readonly struct Result
    {
        public Exception Exception { get; }
        public ActionResult ActionResult { get; }
        public string Message { get; }
        public bool IsSuccess { get; }

        public Result(ActionResult actionResult)
        {
            Exception = null;
            IsSuccess = actionResult != ActionResult.Message;
            Message = string.Empty;
            ActionResult = actionResult;
        }

        public Result(string message)
        {
            Exception = null;
            IsSuccess = false;
            Message = message;
            ActionResult = ActionResult.Message;
        }

        public Result(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            IsSuccess = false;
            Message = exception.Message;
            ActionResult = ActionResult.Exception;
        }

        public bool ErrorIs<TException>() where TException : Exception => Exception is TException;

        public static Result Success() => new(ActionResult.Success);

        public static Result Success(string message) => new(message);

        public static Result<T> Success<T>(T value) => new(value);

        public static Result Error(Exception exception) => new(exception);

        public static Result<T> Error<T>(Exception exception) => new(exception);

        public static implicit operator Result(Exception exception) => new(exception);

        public static implicit operator bool(Result result) => result.IsSuccess;

        public static implicit operator Task<Result>(Result result) => result.AsTask;

        public void Deconstruct(out ActionResult actionResult, out Exception exception) =>
            (actionResult, exception) = (ActionResult, Exception);

        public void Deconstruct(out ActionResult actionResult, out string message, out Exception exception) =>
            (actionResult, message, exception) = (ActionResult, Message, Exception);

        public Task<Result> AsTask => Task.FromResult(this);
    }

    public enum ActionResult
    {
        Success,
        Message,
        Exception
    }
}
