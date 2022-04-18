using FluentResults;

namespace Devit.Commanding;

public class CommandValidationError : Error, ICommandFailure
{
    public string PropertyName { get; }

    public CommandValidationError(string propertyName, string message) : base(message)
    {
        PropertyName = propertyName;
    }
}
