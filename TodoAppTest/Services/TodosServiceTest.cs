using System.Linq.Expressions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client.Events;
using TodoApp.Commands;
using TodoApp.Models;
using TodoApp.Queries;
using TodoApp.Repository;
using TodoApp.Services;
using TodoApp.Utils;
using TodoAppTest.Controllers;

namespace TodoAppTest.Services;

public class TodosServiceTest
{
    private readonly Mock<IRepository> _repository;
    private readonly ITodoService _service;

    public TodosServiceTest()
    {
        Mock<ILogger<TodoService>> logger = new();
        Mock<IRabbitMqService> mqService = new();
        Mock<ISignalRService> signalR = new();
        Mock<ISignalRClientService> signalRClient = new();


        _repository = new Mock<IRepository>();

        //arrange
        mqService
            .Setup(s => s.AddConsumer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((EventingBasicConsumer?)null);
        mqService
            .Setup(s => s.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()));
        signalRClient
            .Setup(s => s.GetHubConnection())
            .Returns((HubConnection?)null);
        signalR
            .Setup(s => s.SendMessage(It.IsAny<string>(), It.IsAny<object>()));

        _service = new TodoService(logger.Object, mqService.Object, signalR.Object,
            signalRClient.Object, _repository.Object);
    }

    [Fact]
    public async void Get()
    {
        //arrange
        var todos = TodosControllerTest.GetTodos();
        var users = todos.Select(todo => new User()
            {
                CreateDate = DateTime.Now,
                DateOfBirth = DateTime.Today,
                Id = todo.CreateUserGuid,
                Name = "User",
                UpdateDate = null,
            })
            .Concat(todos.Select(todo => new User()
            {
                CreateDate = DateTime.Now,
                DateOfBirth = DateTime.Today,
                Id = todo.AssignedUserGuid,
                Name = "User",
                UpdateDate = null,
            }))
            .Distinct()
            .ToList();

        var getQuery = new TodosQuery()
        {
            Page = 1,
            PerPage = 10,
            Total = todos.Count
        };
        _repository
            .Setup(s => s.Get<Todo>(null, getQuery))
            .ReturnsAsync(todos);

        _repository
            .Setup(s => s.Get<User>(It.IsAny<Expression<Func<User, bool>>?>(), It.IsAny<Paging>()))
            .ReturnsAsync(users);

        //act
        var result = await _service.GetAsync(getQuery);

        //assert
        _repository.Verify();
        Assert.NotNull(result);
        Assert.Equal(result.Count, todos.Count);
    }

    [Fact]
    public async void GetById()
    {
        //arrange
        var todo = TodosControllerTest.GetTodos()[0];
        var users = new List<User>()
        {
            new()
            {
                CreateDate = DateTime.Now,
                DateOfBirth = DateTime.Today,
                Id = todo.CreateUserGuid,
                Name = "User",
                UpdateDate = null,
            },
            new()
            {
                CreateDate = DateTime.Now,
                DateOfBirth = DateTime.Today,
                Id = todo.AssignedUserGuid,
                Name = "User",
                UpdateDate = null,
            }
        };

        var getQuery = new TodosQuery()
        {
            Id = Guid.NewGuid()
        };
        _repository
            .Setup(s => s.Get<Todo>(getQuery.Id.Value))
            .ReturnsAsync(todo);

        _repository
            .Setup(s => s.Get<User>(It.IsAny<Expression<Func<User, bool>>?>(), It.IsAny<Paging>()))
            .ReturnsAsync(users);

        //act
        var result = await _service.GetAsync(getQuery.Id.Value);

        //assert
        _repository.Verify();
        Assert.NotNull(result);
        Assert.Equal(result.Name, todo.Name);
        Assert.Equal(result.Description, todo.Description);
    }

    [Fact]
    public async void Add()
    {
        //arrange
        var command = new AddTodoCommand()
        {
            AssignedUserGuid = Guid.NewGuid(),
            CreatedUserGuid = Guid.NewGuid(),
            Description = "Todo 1 Description",
            Name = "Todo 1",
        };

        var users = new List<User>()
        {
            new()
            {
                CreateDate = DateTime.Now,
                DateOfBirth = DateTime.Today,
                Id = command.CreatedUserGuid,
                Name = "User",
                UpdateDate = null,
            },
            new()
            {
                CreateDate = DateTime.Now,
                DateOfBirth = DateTime.Today,
                Id = command.AssignedUserGuid,
                Name = "User",
                UpdateDate = null,
            }
        };

        _repository
            .Setup(s => s.Insert(command.GetTodo()))
            .Returns<Todo>((_) => Task.FromResult(Task.CompletedTask));

        _repository
            .Setup(s => s.Get(It.IsAny<Expression<Func<User, bool>>?>(), It.IsAny<Paging>()))
            .ReturnsAsync(users);

        //act
        var result = await _service.AddAsync(command);

        //assert
        _repository.Verify();
        // mockCommand.Verify();
        Assert.NotNull(result);
        Assert.Equal(result.Name, command.Name);
        Assert.Equal(result.Description, command.Description);
    }

    [Fact]
    public async void Update()
    {
        //arrange
        var todo = TodosControllerTest.GetTodos()[0];
        var users = new List<User>()
        {
            new()
            {
                CreateDate = DateTime.Now,
                DateOfBirth = DateTime.Today,
                Id = todo.CreateUserGuid,
                Name = "User",
                UpdateDate = null,
            },
            new()
            {
                CreateDate = DateTime.Now,
                DateOfBirth = DateTime.Today,
                Id = todo.AssignedUserGuid,
                Name = "User",
                UpdateDate = null,
            }
        };
        var command = new UpdateTodoCommand()
        {
            Description = "Todo 1 Description update",
            Name = "Todo 1 update",
            Complete = true,
            Id = todo.Id,
            Status = TodoStatus.Approved
        };

        _repository
            .Setup(s => s.Get<Todo>(command.Id))
            .ReturnsAsync(todo);

        _repository
            .Setup(s => s.Update(todo))
            .Returns<Todo>((_) => Task.FromResult(Task.CompletedTask));

        _repository
            .Setup(s => s.Get(It.IsAny<Expression<Func<User, bool>>?>(), It.IsAny<Paging>()))
            .ReturnsAsync(users);

        //act
        var result = await _service.UpdateAsync(command);

        //assert
        _repository.Verify();
        Assert.NotNull(result);
        Assert.Equal(result.Id, command.Id);
        Assert.Equal(result.Complete, command.Complete);
        Assert.Equal(result.Status, command.Status);
        Assert.Equal(result.Name, command.Name);
        Assert.Equal(result.Description, command.Description);
    }

    [Fact]
    public async void Delete()
    {
        //arrange
        _repository
            .Setup(s => s.Delete<Todo>(Guid.NewGuid()))
            .Returns<Todo>((_) => Task.FromResult(Task.CompletedTask));

        //act
        await _service.Delete(Guid.NewGuid());

        //assert
        _repository.Verify();
        Assert.True(true);
    }
}
