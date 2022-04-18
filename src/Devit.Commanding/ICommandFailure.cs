namespace Devit.Commanding;

public interface ICommandFailure
{
    string PropertyName { get; }
    string Message { get; }
}
