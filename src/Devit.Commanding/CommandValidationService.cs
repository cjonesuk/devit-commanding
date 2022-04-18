using FluentResults;
using FluentValidation;

namespace Devit.Commanding;

internal sealed class CommandValidationService<TCommand> : ICommandValidationService<TCommand>
{
    private readonly IReadOnlyList<IValidator<TCommand>> _validators;

    public CommandValidationService(IEnumerable<IValidator<TCommand>> validators)
    {
        _validators = validators.ToArray();
    }

    public Result Validate(TCommand command, CommandValidationOptions options)
    {
        if (!_validators.Any())
        {
            return Result.Ok();
        }

        var context = new ValidationContext<TCommand>(command);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(validationResult => validationResult.Errors)
            .Where(f => f != null)
            .ToArray();

        if (failures.Length == 0)
        {
            return Result.Ok();
        }

        if (failures.Any(x => x.Severity == Severity.Error))
        {
            return Result.Fail("Validation failures")
                .WithValidationResults(failures);
        }

        if (failures.Any(x => x.Severity == Severity.Warning))
        {
            if (options.TreatWarningsAsErrors)
            {
                return Result.Fail("Validation failures")
                    .WithValidationResults(failures);
            }
            else
            {
                return Result.Ok()
                    .WithValidationResults(failures);
            }
        }

        return Result.Ok();
    }
}
