using System;
using System.Threading.Tasks;

namespace OperationResult
{
    public readonly struct Result<T>
    {
        public T Value { get; }

        public Exception Exception { get; }

        public bool IsSuccess { get; }

        public Result(T value)
        {
            Value = value;
            Exception = null;
            IsSuccess = true;
        }

        public Result(Exception exception)
        {
            Value = default;
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            IsSuccess = false;
        }

        public static implicit operator Result<T>(T value) => new(value);

        public static implicit operator Result<T>(Exception exception) => new(exception);

        public Result<TEnd> ChangeInAnotherResult<TEnd>(Func<T, TEnd> converter)
            => IsSuccess
                ? new Result<TEnd>(converter(Value))
                : new Result<TEnd>(Exception);

        public Result ChangeInNoResult() =>
            IsSuccess
                ? new Result(true)
                : new Result(Exception);

        public void Deconstruct(out bool isSuccess, out T value)
            => (isSuccess, value) = (IsSuccess, Value);

        public void Deconstruct(out bool isSuccess, out T value, out Exception exception)
            => (isSuccess, value, exception) = (IsSuccess, Value, Exception);

        public static implicit operator bool(Result<T> result)
            => result.IsSuccess;

        public static implicit operator Task<Result<T>>(Result<T> result)
            => result.AsTask;

        public Task<Result<T>> AsTask => Task.FromResult(this);
    }

    public readonly struct Result
    {
        public Exception Exception { get; }

        public bool IsSuccess { get; }

        public Result(bool isSuccess)
        {
            Exception = null;
            IsSuccess = isSuccess;
        }

        public Result(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            IsSuccess = false;
        }

        public bool ErrorIs<TException>() where TException : Exception => Exception is TException;

        public static Result Success() => new(true);

        public static Result<T> Success<T>(T value) => new(value);

        public static Result Error(Exception exception) => new(exception);

        public static Result<T> Error<T>(Exception exception) => new(exception);

        public static implicit operator Result(Exception exception) => new(exception);

        public static implicit operator bool(Result result) => result.IsSuccess;

        public static implicit operator Task<Result>(Result result) => result.AsTask;

        public void Deconstruct(out bool isSuccess, out Exception exception) =>
            (isSuccess, exception) = (IsSuccess, Exception);

        public Task<Result> AsTask => Task.FromResult(this);
    }
}
