using FluentResults;
using MediatR;

namespace Devit.Commanding;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
