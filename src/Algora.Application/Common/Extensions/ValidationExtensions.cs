using FluentValidation;

namespace Algora.Application.Common.Extensions;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeSCNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("SCNumber is required")
            .Length(14).WithMessage("SCNumber must be exactly 14 characters")
            .Matches(@"^\d{14}$").WithMessage("SCNumber must contain only digits");
    }
}



