using System.Linq.Expressions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client.Events;
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
    public async void GetTodo()
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

        var getQuery = new GetTodoQuery()
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
}
