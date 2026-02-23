using WeInsure.Domain.Shared;

namespace WeInsure.Domain.ValueObjects;

public record Address
{
    private Address(string addressLine1, string? addressLine2, string? addressLine3, string postcode)
    {
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        AddressLine3 = addressLine3;
        PostCode = postcode;
    }

    public string AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? AddressLine3 { get; private set; }
    public string PostCode { get; set; }

    public static Result<Address> Create(string addressLine1, string? addressLine2, string? addressLine3, string postcode)
    {

        if (string.IsNullOrWhiteSpace(addressLine1))
        {
            return Result<Address>.Failure(Error.Domain("AddressLine1 is required"));
        }
        
        if (string.IsNullOrWhiteSpace(postcode) || postcode.Length > 8)
        {
            return Result<Address>.Failure(Error.Domain("Postcode is required and must be no longer than 8 characters"));
        }
        
        return  Result.Success(new Address(addressLine1, addressLine2, addressLine3, postcode));
    }
}