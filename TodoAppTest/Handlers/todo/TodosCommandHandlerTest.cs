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

    public TodosCommandHandlerTest()
    {
        _service = new Mock<ITodoService>();
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
            .Returns(Task.FromResult(TodoResponse.GetDto(todoCommand.GetTodo())));

        //act
        var logger = new Mock<ILogger<AddTodoCommandHandler>>();
        var handler = new AddTodoCommandHandler(_service.Object, logger.Object);
        var result = await handler.HandleAsync(todoCommand);

        //assert
        _service.Verify();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Results);
        var data = result.Results[0] as TodoResponse;
        Assert.NotNull(data);
        Assert.Equal(data.Name, todoCommand.Name);
        Assert.Equal(data.Description, todoCommand.Description);
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
        var getDto = new TodoResponse()
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
        Assert.NotEmpty(result.Results);
        var data = result.Results[0] as TodoResponse;
        Assert.NotNull(data);
        Assert.Equal(data.Id, todoCommand.Id);
        Assert.Equal(data.Name, todoCommand.Name);
        Assert.Equal(data.Description, todoCommand.Description);
        Assert.Equal(data.Complete, todoCommand.Complete);
        Assert.Equal(data.Status, todoCommand.Status);
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
        Assert.Empty(result.Results);
    }
}
