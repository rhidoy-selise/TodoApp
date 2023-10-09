using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using MongoDB.Driver;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Models;
using TodoApp.Queries;
using TodoApp.Repository;
using TodoApp.Utils;

namespace TodoApp.Services;

public class TodoService : ITodoService
{
    private readonly ILogger<TodoService> _logger;
    private readonly IRepository _repository;
    private readonly IRabbitMqService _mqService;
    private readonly ISignalRService _signalR;
    private readonly ISignalRClientService _signalRClient;

    private const string TodoExchangeName = "todoExchange";
    private const string TodoRouteAdd = "todo.add";
    private const string TodoRouteUpdate = "todo.update";
    private const string TodoRouteDelete = "todo.delete";

    public TodoService(
        ILogger<TodoService> logger,
        IMongoDatabase database,
        IRabbitMqService mqService,
        ISignalRService signalR,
        ISignalRClientService signalRClient,
        IRepository repository
        )
    {
        _logger = logger;
        _mqService = mqService;
        _signalR = signalR;
        _signalRClient = signalRClient;
        _repository = repository;
        AddConsumer();
        ConnectSocket();
    }

    public async Task<TodoGetDto> CreateAsync(AddTodoCommand dto)
    {
        var entity = dto.GetTodo();
        await _repository.Insert(entity);
        _mqService.SendMessage(TodoExchangeName, TodoRouteAdd, entity);
        await _signalR.SendMessage(TodoRouteAdd, entity);
        return GetDto(entity, null).Result;
    }

    public async Task<List<TodoGetDto>> GetAsync(GetTodoQuery query)
    {
        _logger.Log(LogLevel.Information, "Todo get call");
        var todos = await _repository.Get<Todo>(null, query);

        var userIds = todos.Select(todo => todo.CreateUserGuid)
            .Concat(todos.Select(todo => todo.AssignedUserGuid))
            .Distinct()
            .ToList();

        var users = await GetUsers(userIds);

        return todos
            .Select(todo => GetDto(todo, users).Result)
            .ToList();
    }

    public async Task<TodoGetDto> GetById(Guid id)
    {
        var entity = await _repository.Get<Todo>(id);
        return GetDto(entity, null).Result;
    }

    public async Task DeleteById(Guid id)
    {
        try
        {
            _logger.Log(LogLevel.Information, "Todo delete call {}", id);
            await _repository.Delete<Todo>(id);
            _mqService.SendMessage(TodoExchangeName, TodoRouteDelete, id);
            await _signalR.SendMessage(TodoRouteDelete, id);
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
        }
    }

    public async Task<TodoGetDto> UpdateAsync(UpdateTodoCommand dto)
    {
        try
        {
            _logger.Log(LogLevel.Information, "Todo update call {}", dto);
            var entity = await _repository.Get<Todo>(dto.Id);
            dto.UpdateTodo(entity);

            await _repository.Update(entity);
            _mqService.SendMessage(TodoExchangeName, TodoRouteUpdate, entity);
            await _signalR.SendMessage(TodoRouteUpdate, entity);

            return GetDto(entity, null).Result;
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
            throw;
        }
    }

    private async Task<TodoGetDto> GetDto(Todo entity, List<User>? users)
    {
        if (users == null)
        {
            var userIds = new List<Guid>
            {
                entity.CreateUserGuid,
                entity.AssignedUserGuid
            };
            users = await GetUsers(userIds.Distinct().ToList());
        }

        var dto = TodoGetDto.GetDto(entity);
        dto.CreatedUser = UserGetDto.GetUserDto(users.FirstOrDefault(u => u.Id == entity.CreateUserGuid));
        dto.AssignedUser = UserGetDto.GetUserDto(users.FirstOrDefault(u => u.Id == entity.AssignedUserGuid));

        return dto;
    }

    private Task<List<User>> GetUsers(ICollection<Guid> userIds)
    {
        try
        {
            return _repository
                .Get<User>(u => userIds.Contains(u.Id), new Paging());
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
            throw new Exception(e.Message);
        }
    }

    private void AddConsumer()
    {
        var addConsumer = _mqService.AddConsumer(TodoExchangeName, TodoRouteAdd);
        if (addConsumer != null)
            addConsumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    var decodedString = Encoding.UTF8.GetString(body.ToArray());
                    // var Todo = JsonConvert.DeserializeObject<Todo>(body.ToString());
                    _logger.Log(LogLevel.Information, "Received message for routing key  {}, data {}", TodoRouteAdd,
                        decodedString);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
                }
            };

        var updateConsumer = _mqService.AddConsumer(TodoExchangeName, TodoRouteUpdate);
        if (updateConsumer != null)
            updateConsumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    var decodedString = Encoding.UTF8.GetString(body.ToArray());
                    // var Todo = JsonConvert.DeserializeObject<Todo>(decodedString);
                    _logger.Log(LogLevel.Information, "Received message for routing key  {}, data {}", TodoRouteUpdate,
                        decodedString);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
                }
            };

        var deleteConsumer = _mqService.AddConsumer(TodoExchangeName, TodoRouteDelete);
        if (deleteConsumer != null)
            deleteConsumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    var decodedString = Encoding.UTF8.GetString(body.ToArray());
                    _logger.Log(LogLevel.Information, "Received message for routing key  {}, data {}", TodoRouteDelete,
                        decodedString);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Trace, "Exception found {}", e.Message);
                }
            };
    }

    private void ConnectSocket()
    {
        _signalRClient.GetHubConnection().On<string>(TodoRouteAdd,
            (message) =>
            {
                _logger.Log(LogLevel.Information, "socket message {} on topic {}", message, TodoRouteAdd);
            });

        _signalRClient.GetHubConnection().On<string>(TodoRouteUpdate,
            (message) =>
            {
                _logger.Log(LogLevel.Information, "socket message {} on topic {}", message, TodoRouteUpdate);
            });

        _signalRClient.GetHubConnection().On<string>(TodoRouteDelete,
            (message) =>
            {
                _logger.Log(LogLevel.Information, "socket message {} on topic {}", message, TodoRouteDelete);
            });
    }
}
