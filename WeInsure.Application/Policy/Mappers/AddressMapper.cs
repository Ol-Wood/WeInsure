using WeInsure.Application.Policy.Dtos;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Policy.Mappers;

public static class AddressMapper
{
    public static AddressDto ToDto(this Address address)
    {
        return new AddressDto
        {
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            AddressLine3 = address.AddressLine3,
            PostCode = address.PostCode,
        };
    }
}