using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Entities;

public class InsuredProperty
{
    public Guid Id { get; private set; }
    public Guid PolicyId { get; private set; }
    public Address Address { get; private set; }

    private InsuredProperty(){}
    private InsuredProperty(Guid id, Guid policyId, Address address)
    {
        Id = id;
        PolicyId = policyId;
        Address = address;
    }

    public static InsuredProperty Create(Guid id, Guid policyId, Address address)
    {
        return new InsuredProperty(id, policyId, address);
    }

    public InsuredProperty CopyToNewPolicy(Guid newId, Guid newPolicyId)
    {
        return Create(newId, newPolicyId, Address);
    }
}