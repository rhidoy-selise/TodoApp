using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Queries;
using TodoApp.Utils;

namespace TodoApp.Controllers;

[Authorize]
[Route("api/todos")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly IHandlerService _handler;

    public TodosController(IHandlerService handler)
    {
        _handler = handler;
    }

    [HttpPost("get")]
    public async Task<HandlerResponse> Get(TodosQuery query)
    {
        return await _handler.HandleAsync(query);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddTodoCommand command)
    {
        var result = await _handler.HandleAsync(command);
        var data = ControllerUtil.GetData<TodoResponse>(result);

        return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
    }

    [HttpPost("update")]
    public async Task<TodoResponse> Update(UpdateTodoCommand command)
    {
        var result = await _handler.HandleAsync(command);

        return ControllerUtil.GetData<TodoResponse>(result);
    }

    [HttpPost("delete")]
    public IActionResult Delete(DeleteTodoCommand command)
    {
        _handler.Handle(command);
        return NoContent();
    }
}
