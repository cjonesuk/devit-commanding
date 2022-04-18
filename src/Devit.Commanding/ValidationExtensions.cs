using FluentResults;
using FluentValidation;
using FluentValidation.Results;

namespace Devit.Commanding;

public static class ValidationExtensions
{
    public static Result WithValidationResults(this Result result, IEnumerable<ValidationFailure> failures)
    {
        var reasons = failures.Select(failure => failure.MapToReason());

        result.Reasons.AddRange(reasons);

        return result;
    }

    public static IReason MapToReason(this ValidationFailure failure)
    {
        return failure.Severity switch
        {
            Severity.Error => new CommandValidationError(failure.PropertyName, failure.ErrorMessage),
            Severity.Warning => new CommandValidationWarning(failure.PropertyName, failure.ErrorMessage),
            _ => throw new NotImplementedException()
        };
    }
}
