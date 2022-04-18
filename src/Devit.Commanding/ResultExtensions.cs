using FluentResults;

namespace Devit.Commanding;

internal static class ResultExtensions
{
    public static IEnumerable<T> GetReasons<T>(this ResultBase result)
    {
        return result.Reasons.OfType<T>();
    }

    public static bool HasErrors(this ResultBase result)
    {
        return result.GetReasons<CommandValidationError>().Any();
    }

    public static bool HasWarnings(this ResultBase result)
    {
        return result.GetReasons<CommandValidationWarning>().Any();
    }

    public static Result<TValue> WithWarnings<TValue>(this Result<TValue> input, Result validationResult, CommandValidationOptions options)
    {
        if (!options.TreatWarningsAsErrors && validationResult.HasWarnings())
        {
            input.WithReason(new TreatErrorsAsWarnings());
        }

        return input;
    }
}
