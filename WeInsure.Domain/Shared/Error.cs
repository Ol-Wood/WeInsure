namespace WeInsure.Domain.Shared;

public class Error
{
    private Error(string message, ErrorType type)
    {
        Message = message;
        Type = type;
    }
    public string Message { get;  }
    public ErrorType Type { get; }
    
    public static Error Validation(params string[]? message) => new(string.Join(", ", message), ErrorType.Validation);
    public static Error Domain(string message) => new(message, ErrorType.Domain);
}