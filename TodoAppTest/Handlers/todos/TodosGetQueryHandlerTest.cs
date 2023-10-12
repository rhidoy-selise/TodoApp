using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Dto;
using TodoApp.Queries;
using TodoApp.QueryHandlers;
using TodoApp.Services;
using TodoAppTest.Controllers;

namespace TodoAppTest.Handlers.todos;

public class TodosGetQueryHandlerTest
{
    private readonly Mock<ITodoService> _service;
    private readonly Mock<ILogger<GetTodoQueryHandler>> _logger;

    public TodosGetQueryHandlerTest()
    {
        _service = new Mock<ITodoService>();
        _logger = new Mock<ILogger<GetTodoQueryHandler>>();
    }

    [Fact]
    public async void GetTodo()
    {
        //arrange
        var todos = TodosControllerTest.GetTodos()
            .Select(TodoGetDto.GetDto)
            .ToList();
        var getQuery = new GetTodoQuery()
        {
            Page = 1,
            PerPage = 10,
            Total = todos.Count
        };
        _service
            .Setup(h => h.GetAsync(getQuery))
            .Returns(Task.FromResult(todos));
        var handler = new GetTodoQueryHandler(_service.Object, _logger.Object);

        //act
        var result = await handler.HandleAsync(getQuery);

        //assert
        _service.Verify();
        Assert.NotNull(result);
        Assert.Equal(result.NumberOfElements, todos.Count);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(10, result.PerPage);
        Assert.Equal(result.Content.Count, todos.Count);
    }
}
