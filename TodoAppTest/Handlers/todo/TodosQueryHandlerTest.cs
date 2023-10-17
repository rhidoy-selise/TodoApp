using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Dto;
using TodoApp.Queries;
using TodoApp.QueryHandlers;
using TodoApp.Services;
using TodoAppTest.Controllers;

namespace TodoAppTest.Handlers.todo;

public class TodosQueryHandlerTest
{
    private readonly Mock<ITodoService> _service;
    private readonly TodosQueryHandler _handler;

    public TodosQueryHandlerTest()
    {
        _service = new Mock<ITodoService>();
        Mock<ILogger<TodosQueryHandler>> logger = new();
        _handler = new TodosQueryHandler(_service.Object, logger.Object);
    }

    [Fact]
    public async void Get()
    {
        //arrange
        var todos = TodosControllerTest.GetTodos()
            .Select(TodoResponse.GetDto)
            .ToList();
        var getQuery = new TodosQuery()
        {
            Page = 1,
            PerPage = 10,
            Total = todos.Count
        };
        _service
            .Setup(h => h.GetAsync(getQuery))
            .Returns(Task.FromResult(todos));

        //act
        var result = await _handler.HandleAsync(getQuery);

        //assert
        _service.Verify();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Results);
        var data = result.Results;
        Assert.Equal(data.Count, todos.Count);
    }

    [Fact]
    public async void GetById()
    {
        //arrange
        var todo = TodoResponse.GetDto(TodosControllerTest.GetTodos()[0]);

        var getQuery = new TodosQuery()
        {
            Id = Guid.NewGuid()
        };
        _service
            .Setup(h => h.GetAsync(getQuery))
            .Returns(Task.FromResult(new List<TodoResponse>() { todo }));

        //act
        var result = await _handler.HandleAsync(getQuery);

        //assert
        _service.Verify();
        Assert.NotNull(result);
        Assert.Single(result.Results);
        var data = result.Results[0] as TodoResponse;
        Assert.NotNull(data);
        Assert.Equal(data.Id, todo.Id);
    }
}
