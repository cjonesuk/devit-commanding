using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Devit.Commanding.AspNetCore.Tests;

public class CommandHandlerSetupTests
{
    private readonly IMediator _mediator;

    public CommandHandlerSetupTests()
    {
        var services = (IServiceCollection)new ServiceCollection();

        services.AddCommanding();
        services.AddCommandHandler<TestCommandHandler>();
        services.AddCommandHandler<TestCommandWithResponseHandler>();

        var provider = services.BuildServiceProvider();
        _mediator = provider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task SendingTestCommandReturnsSuccess()
    {
        var command = new TestCommand("Chris", "Jones");
        var result = await _mediator.Send(command);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SendingTestCommandWithResponseReturnsResult()
    {
        var command = new TestCommandWithResponse("Chris", "Jones");
        var result = await _mediator.Send(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Text.Should().Be("Result");

    }
}

internal sealed record TestCommand(string FirstName, string LastName) : ICommand;
internal sealed record TestCommandWithResponse(string FirstName, string LastName) : ICommand<TestCommandResponse>;
internal sealed record TestCommandResponse(string Text);

internal sealed class TestCommandValidator : AbstractValidator<TestCommand>
{
    public TestCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MinimumLength(1).MaximumLength(100);

        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}

internal sealed class TestCommandWithResponseValidator : AbstractValidator<TestCommandWithResponse>
{
    public TestCommandWithResponseValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MinimumLength(1).MaximumLength(100);

        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}

internal sealed class TestCommandHandler : ICommandHandler<TestCommand>
{
    public async Task<Result> Handle(TestCommand request, CancellationToken cancellationToken)
    {
        return Result.Ok();
    }
}

internal sealed class TestCommandWithResponseHandler : ICommandHandler<TestCommandWithResponse, TestCommandResponse>
{
    public async Task<Result<TestCommandResponse>> Handle(TestCommandWithResponse request, CancellationToken cancellationToken)
    {
        return Result.Ok(new TestCommandResponse("Result"));
    }
}