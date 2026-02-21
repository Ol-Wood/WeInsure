namespace WeInsure.Domain.Shared;

public class Result<T>
{
    public T? Data { get; private set; }
    public Error? Error { get; private set; }
    private Result(){}

    public static Result<T> Success(T value)
    {
        return new Result<T> { Data = value };
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T> { Error = error };
    }
    
    public bool IsSuccess => Data != null && Error == null;
}

public class Result
{
    public Error? Error { get; private set; }
    private Result(){}

    public static Result Success()
    {
        return new Result();
    }

    public static Result Failure(Error error)
    {
        return new Result { Error = error };
    }
    
    public bool IsSuccess => Error == null;
}