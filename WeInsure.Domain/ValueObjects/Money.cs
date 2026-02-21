using WeInsure.Domain.Shared;

namespace WeInsure.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; private set; }
    
    
    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Result<Money> Create(decimal amount)
    {
        if (amount < 0)
        {
            return Result<Money>.Failure(Error.Domain("Amount cannot be negative."));
        }

        if (decimal.Round(amount, 2) != amount)
            return Result<Money>.Failure(Error.Domain("Amount cannot have more than 2 decimal places."));
        
        return Result<Money>.Success(new Money(amount));
    }
    
}