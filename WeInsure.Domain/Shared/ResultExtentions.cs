using WeInsure.Domain.Exceptions;

namespace WeInsure.Domain.Shared;

public static class ResultExtensions
{
    public static T OrThrow<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Data;
        }
        
        var exception = result.Error.ConvertErrorToException();
        throw exception;
    }


    private static Exception ConvertErrorToException(this Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => new ValidationException(error.Message),
            ErrorType.Domain => new DomainException(error.Message),
            _ => new Exception(error.Message)
        };
    }
}