using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApp.Commands;
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
    public async void Get()
    {
        //arrange
        var todos = GetTodos()
            .Select(TodoResponse.GetDto)
            .ToList();
        var getQuery = new TodosQuery()
        {
            Page = 1,
            PerPage = 10,
            Total = todos.Count
        };
        _handler
            .Setup(h => h.HandleAsync(getQuery))
            .Returns(Task.FromResult(new HandlerResponse(todos)));
        var controller = new TodosController(_handler.Object);

        //act
        var result = await controller.Get(getQuery);

        //assert
        _handler.Verify();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Results);
        Assert.Equal(result.Results.Count, todos.Count);
    }

    [Fact]
    public async void GetById()
    {
        //arrange
        var todo = TodoResponse.GetDto(GetTodos()[0]);
        var getQuery = new TodosQuery()
        {
            Id = Guid.NewGuid()
        };
        _handler
            .Setup(h => h.HandleAsync(getQuery))
            .Returns(Task.FromResult(new HandlerResponse(new List<object> { todo })));
        var controller = new TodosController(_handler.Object);

        //act
        var result = await controller.Get(getQuery);

        //assert
        _handler.Verify();
        Assert.NotNull(result);
        Assert.Single(result.Results);
        var data = result.Results[0] as TodoResponse;
        Assert.NotNull(data);
        Assert.Equal(data.Id, todo.Id);
        Assert.Equal(data.Name, todo.Name);
        Assert.Equal(data.Description, todo.Description);
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

        _handler
            .Setup(h => h.HandleAsync(command))
            .Returns(Task.FromResult(new HandlerResponse(new List<object> { TodoResponse.GetDto(command.GetTodo()) })));
        var controller = new TodosController(_handler.Object);

        //act
        var result = await controller.Add(command);
        var objectResult = result as ObjectResult;

        //assert
        _handler.Verify();
        Assert.NotNull(result);
        Assert.NotNull(objectResult);
        if (objectResult == null) return;
        Assert.Equal(objectResult.StatusCode, 201);
        var data = objectResult.Value as TodoResponse;
        Assert.NotNull(data);
        Assert.Equal(data.Name, command.Name);
        Assert.Equal(data.Description, command.Description);
    }

    [Fact]
    public async void Update()
    {
        //arrange
        var command = new UpdateTodoCommand()
        {
            Description = "Todo 1 Description",
            Complete = true,
            Id = Guid.NewGuid(),
            Name = "Todo 1",
            Status = TodoStatus.Approved
        };
        var getDto = new TodoResponse()
        {
            AssignedUser = null,
            Complete = command.Complete,
            CreateDate = DateTime.Now,
            UpdateDate = null,
            CreatedUser = null,
            Description = command.Description,
            Id = command.Id,
            Name = command.Name,
            Status = command.Status
        };

        _handler
            .Setup(h => h.HandleAsync(command))
            .Returns(Task.FromResult(new HandlerResponse(new List<object> { getDto })));
        var controller = new TodosController(_handler.Object);

        //act
        var result = await controller.Update(command);

        //assert
        _handler.Verify();
        Assert.NotNull(result);
        Assert.Equal(result.Id, command.Id);
        Assert.Equal(result.Name, command.Name);
        Assert.Equal(result.Description, command.Description);
        Assert.Equal(result.Complete, command.Complete);
        Assert.Equal(result.Status, command.Status);
    }

    [Fact]
    public void Delete()
    {
        //arrange
        var command = new DeleteTodoCommand()
        {
            Id = Guid.NewGuid()
        };

        _handler
            .Setup(h => h.Handle(command))
            .Returns(new HandlerResponse(null));
        var controller = new TodosController(_handler.Object);

        //act
        var result = controller.Delete(command);

        //assert
        _handler.Verify();
        Assert.NotNull(result);
        var objectResult = result as NoContentResult;
        Assert.NotNull(objectResult);
        if (objectResult == null) return;
        Assert.Equal(204, objectResult.StatusCode);
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
