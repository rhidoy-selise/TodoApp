using TodoApp.Dto;

namespace TodoApp.Utils;

public class Handler : IHandlerService
{
    private readonly IServiceProvider _serviceProvider;

    public Handler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private IHandler<TSignal> GetHandler<TSignal>(TSignal signal)
    {
        if (signal == null)
        {
            throw new ArgumentNullException(nameof(signal));
        }

        var handlerType = typeof(IHandler<TSignal>);

        if (_serviceProvider.GetService(handlerType) is not IHandler<TSignal> handler)
        {
            throw new InvalidOperationException(
                $"No handler found for type {signal.GetType()}");
        }

        return handler;
    }

    public HandlerResponse Handle<TSignal>(TSignal signal)
    {
        var handler = GetHandler(signal);
        return handler.Handle(signal);
    }

    public async Task<HandlerResponse> HandleAsync<TSignal>(TSignal signal)
    {
        var handler = GetHandler(signal);
        return await handler.HandleAsync(signal);
    }
}

public interface IHandlerService
{
    HandlerResponse Handle<TSignal>(TSignal signal);
    Task<HandlerResponse> HandleAsync<TSignal>(TSignal signal);
}

public interface IHandler<in TSignal>
{
    HandlerResponse Handle(TSignal signal);
    Task<HandlerResponse> HandleAsync(TSignal signal);
}
