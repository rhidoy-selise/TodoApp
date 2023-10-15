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
            .Select(TodoGetDto.GetDto)
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
        Assert.Equal(result.NumberOfElements, todos.Count);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(10, result.PerPage);
        Assert.Equal(result.Content.Count, todos.Count);
    }

    [Fact]
    public async void GetById()
    {
        //arrange
        var todo = TodoGetDto.GetDto(TodosControllerTest.GetTodos()[0]);

        var getQuery = new TodosQuery()
        {
            Id = Guid.NewGuid()
        };
        _service
            .Setup(h => h.GetAsync(getQuery))
            .Returns(Task.FromResult(new List<TodoGetDto>(){todo}));

        //act
        var result = await _handler.HandleAsync(getQuery);

        //assert
        _service.Verify();
        Assert.NotNull(result);
        Assert.NotNull(result);
        Assert.NotNull(result.Content);
        Assert.Equal(1, result.NumberOfElements);
        Assert.Single(result.Content);
    }
}
