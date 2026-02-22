namespace WeInsure.Domain.ValueObjects;

public class PolicyReference
{
    private const string Prefix = "POL";
    public string Value { get; private set; }

    private PolicyReference(string value)
    {
        Value = value;
    }

    public static PolicyReference Create()
    {
        var dateTimePart = DateTime.UtcNow.ToString("yyMMddH");
        var randomSuffix = Guid.NewGuid().ToString("N")[..6].ToUpper();
        var reference = $"{Prefix}-{dateTimePart}-{randomSuffix}";
        
        return new PolicyReference(reference);
    }
}