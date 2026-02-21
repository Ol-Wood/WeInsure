namespace WeInsure.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; private set; }
    
    
    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Money Create(decimal amount)
    {
        return new Money(amount);
    }
    
}