using FluentResults;

namespace Devit.Commanding;

public class TreatErrorsAsWarnings : Error
{
    public TreatErrorsAsWarnings() : base("Treating warnings as errors")
    {

    }
}
