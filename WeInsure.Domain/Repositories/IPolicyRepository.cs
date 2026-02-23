using WeInsure.Domain.Entities;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Repositories;

public interface IPolicyRepository
{
    Task<Policy> Add(Policy policy);
    Task<bool> Exists(PolicyReference reference);
    Task<Policy?> GetByReference(string reference);
}