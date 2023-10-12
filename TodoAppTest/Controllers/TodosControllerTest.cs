using Moq;
using TodoApp.Controllers;
using TodoApp.Dto;
using TodoApp.Models;
using TodoApp.Queries;
using TodoApp.Utils;

namespace TodoAppTest.Controllers;

public class TodosControllerTest
{
    private readonly Mock<IHandlerService> _handler;

    public TodosControllerTest()
    {
        _handler = new Mock<IHandlerService>();
    }

    [Fact]
    public async void GetTodo()
    {
        //arrange
        var todos = GetTodos()
            .Select(TodoGetDto.GetDto)
            .ToList();
        var getQuery = new GetTodoQuery()
        {
            Page = 1,
            PerPage = 10,
            Total = todos.Count
        };
        _handler
            .Setup(h => h.HandleAsync<GetTodoQuery, PagedList<TodoGetDto>>(getQuery))
            .Returns(Task.FromResult(PagedList<TodoGetDto>.GetPagedList(todos, getQuery)));
        var controller = new TodosController(_handler.Object);

        //act
        var result = await controller.Get(getQuery);

        //assert
        _handler.Verify();
        Assert.NotNull(result);
        Assert.Equal(result.NumberOfElements, todos.Count);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(10, result.PerPage);
        Assert.Equal(result.Content.Count, todos.Count);
    }

    public static List<Todo> GetTodos()
    {
        var todos = new List<Todo>
        {
            new()
            {
                AssignedUserGuid = Guid.NewGuid(),
                Complete = false,
                CreateDate = DateTime.Now,
                CreateUserGuid = Guid.NewGuid(),
                Description = "Todo 1 More",
                Id = Guid.NewGuid(),
                Name = "Todo 1",
                Status = TodoStatus.Pending,
                UpdateDate = null
            },
            new()
            {
                AssignedUserGuid = Guid.NewGuid(),
                Complete = true,
                CreateDate = DateTime.Now,
                CreateUserGuid = Guid.NewGuid(),
                Description = "Todo 2 More",
                Id = Guid.NewGuid(),
                Name = "Todo 2",
                Status = TodoStatus.Approved,
                UpdateDate = null
            },
            new()
            {
                AssignedUserGuid = Guid.NewGuid(),
                Complete = false,
                CreateDate = DateTime.Now,
                CreateUserGuid = Guid.NewGuid(),
                Description = "Todo 3 More",
                Id = Guid.NewGuid(),
                Name = "Todo 3",
                Status = TodoStatus.Pending,
                UpdateDate = null
            }
        };
        return todos;
    }
}
