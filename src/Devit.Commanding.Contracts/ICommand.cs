using FluentResults;
using MediatR;

namespace Devit.Commanding;

public interface ICommand : IRequest<Result>
{

}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

