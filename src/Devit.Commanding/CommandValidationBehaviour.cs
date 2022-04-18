using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Devit.Commanding;

public class CommandValidationBehaviour<TCommand, TResponse> : IPipelineBehavior<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
    private readonly ICommandValidationService<TCommand> _validationService;
    private readonly ILogger<TCommand> _logger;

    public CommandValidationBehaviour(ICommandValidationService<TCommand> validationService, ILogger<TCommand> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken, RequestHandlerDelegate<Result<TResponse>> next)
    {
        var options = new CommandValidationOptions(true);
        var validationResult = _validationService.Validate(command, options);

        if (validationResult.IsFailed)
        {
            return validationResult;
        }

        var handlerResult = await next();

        handlerResult.WithWarnings(validationResult, options);

        return handlerResult;
    }
}

