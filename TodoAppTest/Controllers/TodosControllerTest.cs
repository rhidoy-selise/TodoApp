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
            .Select(TodoGetDto.GetDto)
            .ToList();
        var getQuery = new TodosQuery()
        {
            Page = 1,
            PerPage = 10,
            Total = todos.Count
        };
        _handler
            .Setup(h => h.HandleAsync<TodosQuery, PagedList<TodoGetDto>>(getQuery))
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

    [Fact]
    public async void GetById()
    {
        //arrange
        var todo = TodoGetDto.GetDto(GetTodos()[0]);
        var getQuery = new TodosQuery()
        {
            Id = Guid.NewGuid()
        };
        _handler
            .Setup(h => h.HandleAsync<TodosQuery, PagedList<TodoGetDto>>(getQuery))
            .Returns(Task.FromResult(PagedList<TodoGetDto>.GetPagedList(new List<TodoGetDto>() { todo }, getQuery)));
        var controller = new TodosController(_handler.Object);

        //act
        var result = await controller.Get(getQuery);

        //assert
        _handler.Verify();
        Assert.NotNull(result);
        Assert.NotNull(result.Content);
        Assert.Equal(1, result.NumberOfElements);
        Assert.Single(result.Content);
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
            .Setup(h => h.HandleAsync<AddTodoCommand, TodoGetDto>(command))
            .Returns(Task.FromResult(TodoGetDto.GetDto(command.GetTodo())));
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
        var value = objectResult.Value;
        Assert.NotNull(value);
        switch (value)
        {
            case null:
                return;
            case TodoGetDto dto:
                Assert.Equal(dto.Name, command.Name);
                Assert.Equal(dto.Description, command.Description);
                break;
        }
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
        var getDto = new TodoGetDto()
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
            .Setup(h => h.HandleAsync<UpdateTodoCommand, TodoGetDto>(command))
            .Returns(Task.FromResult(getDto));
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
            .Setup(h => h.Handle<DeleteTodoCommand, Task>(command))
            .Returns(Task.FromResult(Task.CompletedTask));
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
