using FluentResults;

namespace Devit.Commanding;

public interface ICommandValidationService<TCommand>
{
    Result Validate(TCommand command, CommandValidationOptions options);
}
