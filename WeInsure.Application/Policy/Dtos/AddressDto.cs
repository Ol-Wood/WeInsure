namespace WeInsure.Application.Policy.Dtos;

public record AddressDto
{
    public string AddressLine1 { get; init; }
    public string AddressLine2 { get; init; }
    public string AddressLine3 { get; init; }
    public string PostCode { get; init; }
}