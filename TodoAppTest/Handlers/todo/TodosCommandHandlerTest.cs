using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.CommandHandlers;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoAppTest.Handlers.todo;

public class TodosCommandHandlerTest
{
    private readonly Mock<ITodoService> _service;
    public TodosCommandHandlerTest(Mock<ITodoService> service)
    {
        _service = service;
    }

    [Fact]
    public async void Add()
    {
        //arrange
        var todoCommand = new AddTodoCommand()
        {
            AssignedUserGuid = Guid.NewGuid(),
            CreatedUserGuid = Guid.NewGuid(),
            Description = "Todo 1 Description",
            Name = "Todo 1",
        };

        _service
            .Setup(h => h.AddAsync(todoCommand))
            .Returns(Task.FromResult(TodoGetDto.GetDto(todoCommand.GetTodo())));

        //act
        var logger = new Mock<ILogger<AddTodoCommandHandler>>();
        var handler = new AddTodoCommandHandler(_service.Object, logger.Object);
        var result = await handler.HandleAsync(todoCommand);

        //assert
        _service.Verify();
        Assert.NotNull(result);
        Assert.Equal(result.Name, todoCommand.Name);
        Assert.Equal(result.Description, todoCommand.Description);
    }

    [Fact]
    public async void Update()
    {
        //arrange
        var todoCommand = new UpdateTodoCommand()
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
            Complete = todoCommand.Complete,
            CreateDate = DateTime.Now,
            UpdateDate = null,
            CreatedUser = null,
            Description = todoCommand.Description,
            Id = todoCommand.Id,
            Name = todoCommand.Name,
            Status = todoCommand.Status
        };

        _service
            .Setup(h => h.UpdateAsync(todoCommand))
            .Returns(Task.FromResult(getDto));

        //act
        var logger = new Mock<ILogger<UpdateTodoCommandHandler>>();
        var handler = new UpdateTodoCommandHandler(_service.Object, logger.Object);
        var result = await handler.HandleAsync(todoCommand);

        //assert
        _service.Verify();
        Assert.NotNull(result);
        Assert.Equal(result.Id, todoCommand.Id);
        Assert.Equal(result.Name, todoCommand.Name);
        Assert.Equal(result.Description, todoCommand.Description);
        Assert.Equal(result.Complete, todoCommand.Complete);
        Assert.Equal(result.Status, todoCommand.Status);
    }

    [Fact]
    public async void Delete()
    {
        //arrange
        var todoCommand = new DeleteTodoCommand()
        {
            Id = Guid.NewGuid()
        };

        _service
            .Setup(h => h.Delete(todoCommand.Id))
            .Returns(Task.FromResult(Task.CompletedTask));

        //act
        var logger = new Mock<ILogger<DeleteTodoCommandHandler>>();
        var handler = new DeleteTodoCommandHandler(_service.Object, logger.Object);
        var result = await handler.HandleAsync(todoCommand);

        //assert
        _service.Verify();
        Assert.NotNull(result);
    }
}
