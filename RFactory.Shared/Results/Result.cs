namespace RFactory.Shared.Results;

/// <summary>
/// Lightweight operation result used by the Application layer to signal success/failure
/// without throwing for expected business outcomes.
/// </summary>
public class Result
{
    public bool Succeeded { get; protected set; }
    public string? Error { get; protected set; }

    protected Result(bool succeeded, string? error)
    {
        Succeeded = succeeded;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(bool succeeded, T? data, string? error) : base(succeeded, error)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static new Result<T> Failure(string error) => new(false, default, error);
}
