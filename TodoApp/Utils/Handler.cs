namespace TodoApp.Utils;

public class Handler : IHandlerService
{
    private readonly IServiceProvider _serviceProvider;

    public Handler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private IHandler<TQuery, TQueryResponse> GetHandler<TQuery, TQueryResponse>(TQuery query)
    {
        if (query == null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var handlerType = typeof(IHandler<TQuery, TQueryResponse>);

        if (_serviceProvider.GetService(handlerType) is not IHandler<TQuery, TQueryResponse> handler)
        {
            throw new InvalidOperationException(
                $"No handler found for type {query.GetType()} and response {typeof(TQueryResponse)}");
        }

        return handler;
    }

    public async Task<TQueryResponse> HandleAsync<TQuery, TQueryResponse>(TQuery query)
    {
        var handler = GetHandler<TQuery, TQueryResponse>(query);
        return await handler.HandleAsync(query);
    }

    public TQueryResponse Handle<TQuery, TQueryResponse>(TQuery query)
    {
        var handler = GetHandler<TQuery, TQueryResponse>(query);
        return handler.Handle(query);
    }
}

public interface IHandlerService
{
    Task<TQueryResponse> HandleAsync<TQuery, TQueryResponse>(TQuery query);
    TQueryResponse Handle<TQuery, TQueryResponse>(TQuery query);
}

public interface IHandler<in TQuery, TQueryResponse>
{
    TQueryResponse Handle(TQuery query);
    Task<TQueryResponse> HandleAsync(TQuery query);
}
