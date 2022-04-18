using FluentResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Devit.Commanding.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommanding(this IServiceCollection services)
    {
        var configuration = new MediatRServiceConfiguration()
            .AsSingleton();

        MediatR.Registration.ServiceRegistrar.AddRequiredServices(services, configuration);

        return services;
    }

    public static IServiceCollection AddCommandHandler<THandler>(this IServiceCollection services)
    where THandler : class
    {
        var handlerType = typeof(THandler);

        RegisterCommandHandler(services, handlerType);
        RegisterCommandResponseHandler(services, handlerType);

        return services;
    }

    private static void RegisterCommandHandler(IServiceCollection services, Type handlerType)
    {
        var interfaces = handlerType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

        foreach (var @interface in interfaces)
        {
            var commandType = @interface.GetGenericArguments().First();

            var handlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(commandType, typeof(Result));

            services.AddTransient(handlerInterfaceType, handlerType);
        }
    }

    private static void RegisterCommandResponseHandler(IServiceCollection services, Type handlerType)
    {
        var interfaces = handlerType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

        foreach (var @interface in interfaces)
        {
            var commandType = @interface.GetGenericArguments()[0];
            var responseType = @interface.GetGenericArguments()[1];

            var resultType = typeof(Result<>).MakeGenericType(responseType);
            var handlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(commandType, resultType);

            services.AddTransient(handlerInterfaceType, handlerType);
        }
    }

    //public static IServiceCollection AddCommandHandler<TCommand, THandler>(this IServiceCollection services)
    //    where TCommand : ICommand
    //    where THandler : class, ICommandHandler<TCommand>
    //{
    //    services.AddTransient<IRequestHandler<TCommand, Result>, THandler>();

    //    return services;
    //}

    //public static IServiceCollection AddCommandHandler<TCommand, TResponse, THandler>(this IServiceCollection services)
    //    where TCommand : ICommand<TResponse>
    //    where THandler : class, ICommandHandler<TCommand, TResponse>
    //{
    //    services.AddTransient<IRequestHandler<TCommand, Result<TResponse>>, THandler>();

    //    return services;
    //}
}

