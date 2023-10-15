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
    public async Task<PagedList<TodoGetDto>> Get(TodosQuery query)
    {
        return await _handler.HandleAsync<TodosQuery, PagedList<TodoGetDto>>(query);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddTodoCommand command)
    {
        var todo = await _handler.HandleAsync<AddTodoCommand, TodoGetDto>(command);
        return Created("getById", todo);
    }

    [HttpPost("update")]
    public async Task<TodoGetDto> Update(UpdateTodoCommand command)
    {
        return await _handler.HandleAsync<UpdateTodoCommand, TodoGetDto>(command);
    }

    [HttpPost("delete")]
    public IActionResult Delete(DeleteTodoCommand command)
    {
        _handler.Handle<DeleteTodoCommand, Task>(command);
        return NoContent();
    }
}
