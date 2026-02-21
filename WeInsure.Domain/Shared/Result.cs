using System.Diagnostics.CodeAnalysis;

namespace WeInsure.Domain.Shared;

public class Result
{
    protected Result(bool isSuccess) => IsSuccess = isSuccess;
    public Error? Error { get; protected init; }
    public static Result Success() => new(true);
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result Failure(Error error) => new(false) { Error = error };
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
    
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool IsSuccess { get; }
}

public class Result<T> : Result
{
    private Result(bool isSuccess) : base(isSuccess) { }

    [MemberNotNullWhen(true, nameof(Data))]
    public override bool IsSuccess => base.IsSuccess;
    
    public T? Data { get; private init; }
    
    public static Result<T> Success(T value) => new(true) { Data = value };
    public new static Result<T> Failure(Error error) => new(false) { Error = error };
}