using WeInsure.Domain.ValueObjects;

namespace WeInsure.Application.Services;

public interface IPolicyReferenceGenerator
{
    Task<PolicyReference> Generate();
}