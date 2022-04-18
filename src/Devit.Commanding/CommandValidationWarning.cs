using FluentResults;

namespace Devit.Commanding;

public class CommandValidationWarning : Error, ICommandFailure
{
    public string PropertyName { get; }

    public CommandValidationWarning(string propertyName, string message) : base(message)
    {
        PropertyName = propertyName;
    }
}
