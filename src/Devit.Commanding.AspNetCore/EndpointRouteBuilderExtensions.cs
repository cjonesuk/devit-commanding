using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace Devit.Commanding.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapCommand<TCommand>(this IEndpointRouteBuilder builder, string pattern) where TCommand : ICommand
    {
        var action = async (
             [FromServices] IMediator mediator,
             [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] TCommand command,
             CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        };

        return builder.MapPost(pattern, action);
    }

    public static RouteHandlerBuilder MapCommand<TCommand, TResponse>(this IEndpointRouteBuilder builder, string pattern) where TCommand : ICommand<TResponse>
    {
        var action = async (
             [FromServices] IMediator mediator,
             [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] TCommand command,
             CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToActionResult();
        };

        return builder.MapPost(pattern, action);
    }
}
