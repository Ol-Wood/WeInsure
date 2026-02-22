using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Entities;

public class Property
{
    public Guid Id { get; private set; }
    public Guid PolicyId { get; private set; }
    public Address Address { get; private set; }

    private Property(Guid id, Guid policyId, Address address)
    {
        Id = id;
        PolicyId = policyId;
        Address = address;
    }

    public static Property Create(Guid id, Guid policyId, Address address)
    {
        return new Property(id, policyId, address);
    }
}