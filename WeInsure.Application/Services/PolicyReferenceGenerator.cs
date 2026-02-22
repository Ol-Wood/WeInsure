using WeInsure.Application.Exceptions;
using WeInsure.Domain.Repositories;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Services;

public class PolicyReferenceGenerator(IPolicyRepository policyRepository) : IPolicyReferenceGenerator
{
    private const int GenerationRetryCount = 3;

    public async Task<PolicyReference> Generate()
    {
        return await GenerateWithRetry(GenerationRetryCount);
    }

    private async Task<PolicyReference> GenerateWithRetry(int retryCount)
    {
        while (true)
        {
            
            if (retryCount == 0)
            {
                break;
            }
            
            var policyReference = PolicyReference.Create();
            if (await policyRepository.Exists(policyReference))
            {
                retryCount = --retryCount;
                continue;
            }

            return policyReference;
        }
        
        throw new PolicyReferenceGenerationException(
            "Could not generate a unique policy reference. Max retry count exceeded.");
    }
}