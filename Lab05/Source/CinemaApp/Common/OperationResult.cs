namespace CinemaApp.Common;

public sealed class OperationResult<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string Message { get; }

    private OperationResult(bool isSuccess, T? value, string message)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
    }

    public static OperationResult<T> Success(T value, string message = "Операцію виконано успішно.") =>
        new(true, value, message);

    public static OperationResult<T> Failure(string message) =>
        new(false, default, message);
}