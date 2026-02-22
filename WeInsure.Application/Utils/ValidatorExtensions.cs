using FluentValidation;
using FluentValidation.Results;
using WeInsure.Domain.Shared;

namespace WeInsure.Application.Utils;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptionsConditions<T, TProperty> MustBeSuccessfulResult<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder,
        Func<TProperty, Result> resultFactory)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            var result = resultFactory(value);
            if (!result.IsSuccess)
                context.AddFailure(result.Error.Message);
        });
    }


    public static Error ToValidationError(this ValidationResult validationResult)
    {
        return Error.Validation(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());
    }
}