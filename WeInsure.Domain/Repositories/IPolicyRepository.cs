using WeInsure.Domain.Entities;

namespace WeInsure.Domain.Repositories;

public interface IPolicyRepository
{
    Task<Policy> Add(Policy policy);
}