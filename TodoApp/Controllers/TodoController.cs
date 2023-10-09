using Microsoft.AspNetCore.Mvc;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Queries;
using TodoApp.Utils;

namespace TodoApp.Controllers;

[Route("api/todos")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly Handler _handler;

    public TodosController(Handler handler)
    {
        _handler = handler;
    }

    [HttpPost("get")]
    public async Task<PagedList<TodoGetDto>> Get(GetTodoQuery query)
    {
        return await _handler.HandleAsync<GetTodoQuery, PagedList<TodoGetDto>>(query);
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

    [HttpPost("getById")]
    public async Task<TodoGetDto> GetById(GetTodoByIdQuery query)
    {
        return await _handler.HandleAsync<GetTodoByIdQuery, TodoGetDto>(query);
    }

    [HttpPost("delete")]
    public IActionResult Delete(DeleteTodoCommand command)
    {
        _handler.Handle<DeleteTodoCommand, Task>(command);
        return NoContent();
    }
}
