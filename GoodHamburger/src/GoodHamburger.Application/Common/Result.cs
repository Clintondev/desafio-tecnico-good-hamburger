namespace GoodHamburger.Application.Common;

public enum ErrorCode
{
    None,
    NotFound,
    InvalidInput
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ErrorCode ErrorCode { get; }

    private Result(bool isSuccess, T? value, string? error, ErrorCode errorCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T value) => new(true, value, null, ErrorCode.None);
    public static Result<T> Failure(string error, ErrorCode errorCode = ErrorCode.InvalidInput) =>
        new(false, default, error, errorCode);
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ErrorCode ErrorCode { get; }

    private Result(bool isSuccess, string? error, ErrorCode errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true, null, ErrorCode.None);
    public static Result Failure(string error, ErrorCode errorCode = ErrorCode.InvalidInput) =>
        new(false, error, errorCode);
}
