using Microsoft.AspNetCore.Mvc;
using TodoApp.Dto;
using TodoApp.Services;

namespace TodoApp.Controllers;

[Route("api/todos")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodosController> _logger;

    public TodosController(
        ILogger<TodosController> logger,
        ITodoService todoService)
    {
        _logger = logger;
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<List<TodoGetDto>> Get(int page = 1, int pageSize = 10)
    {
        _logger.Log(LogLevel.Information, "Todo get call");
        return await _todoService.GetAsync(page, pageSize);
    }

    [HttpPost]
    public async Task<IActionResult> Add(TodoCreateDto dto)
    {
        var response = new Response();
        try
        {
            await _todoService.CreateAsync(dto);
            response.IsSuccess = true;
            response.Message = "Success";
            _logger.Log(LogLevel.Information, "Todo added {}", dto);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = "Exception Occurs : " + ex.Message;
        }

        return Created("todo", response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        TodoUpdateDto dto
    )
    {
        var response = new Response();
        try
        {
            dto.Id = id;
            var entity = await _todoService.UpdateAsync(dto);
            response.IsSuccess = true;
            response.Message = "Success";
            _logger.Log(LogLevel.Information, "Todo updated {}", entity);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = "Exception Occurs : " + ex.Message;
        }

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<TodoGetDto> GetById(
        [FromRoute] Guid id
    )
    {
        return await _todoService.GetById(id);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id
    )
    {
        var response = new Response();
        try
        {
            await _todoService.DeleteById(id);
            response.IsSuccess = true;
            response.Message = "Success";
            _logger.Log(LogLevel.Information, "Todo deleted {}", id);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = "Exception Occurs : " + ex.Message;
        }

        return Ok(response);
    }
}
