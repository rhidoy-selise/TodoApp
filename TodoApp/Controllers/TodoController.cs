using Microsoft.AspNetCore.Mvc;
using TodoApp.Dto;
using TodoApp.Services;

namespace TodoApp.Controllers;

[Route("api/[controller]/[Action]")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly TodoService _todoService;
    private readonly ILogger<TodosController> _logger;

    public TodosController(
        ILogger<TodosController> logger,
        TodoService todoService)
    {
        _logger = logger;
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<List<TodoGetDto>> Get(int page, int pageSize)
    {
        return await _todoService.GetAsync(page, pageSize);
    }

    [HttpPost]
    public async Task<IActionResult> Add(TodoCreateDto dto)
    {
        var response = new ResponseDTO();
        try
        {
            await _todoService.CreateAsync(dto);
            response.IsSuccess = true;
            response.Message = "Success";
            _logger.Log(LogLevel.Trace, "Todo added {}", dto);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = "Exception Occurs : " + ex.Message;
        }

        return Created("todo", response);
    }
}
