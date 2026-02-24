using Microsoft.EntityFrameworkCore;
using WeInsure.Domain.Entities;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Data.Repositories;

public class PolicyRepository(WeInsureDbContext dbContext) : IPolicyRepository
{
    public async Task<Policy> Add(Policy policy)
    {
        dbContext.Policies.Add(policy);
        await dbContext.SaveChangesAsync();
        return policy;
    }

    public async Task<bool> Exists(string reference)
    {
       return await dbContext.Policies.AnyAsync(p => p.Reference.Equals(reference));
    }

    public Task<Policy?> GetByReference(string reference)
    {
        return dbContext.Policies
            .Include(p => p.InsuredProperty)
            .Include(p => p.Payment)
            .Include(p => p.PolicyHolders)
            .FirstOrDefaultAsync(p => p.Reference == PolicyReference.FromExisting(reference));
    }
}