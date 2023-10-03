using Microsoft.AspNetCore.Mvc;
using TodoApp.Dto;
using TodoApp.Services;

namespace TodoApp.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _service;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        ILogger<UsersController> logger,
        UserService userService)
    {
        _logger = logger;
        _service = userService;
    }

    [HttpGet("")]
    public async Task<List<UserGetDto?>> Get(int page = 1, int pageSize = 10)
    {
        //get users
        var users = await _service.GetAsync(page, pageSize);

        //convert to dto
        var tasks = users.Select(u => Task.FromResult(UserGetDto.GetUserDto(u)));

        //return when complete the processing
        return (await Task.WhenAll(tasks)).ToList();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add(UserCreateDto user)
    {
        var response = new ResponseDTO();
        try
        {
            await _service.CreateAsync(user);
            response.IsSuccess = true;
            response.Message = "Success";
            _logger.Log(LogLevel.Information, "User added {}", user);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = "Exception Occurs : " + ex.Message;
        }

        return Created("user", response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        UserUpdateDto dto
    )
    {
        var response = new ResponseDTO();
        try
        {
            dto.Id = id;
            var user = await _service.UpdateAsync(dto);
            response.IsSuccess = true;
            response.Message = "Success";
            _logger.Log(LogLevel.Information, "User updated {}", user);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = "Exception Occurs : " + ex.Message;
        }

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<UserGetDto?> GetById(
        [FromRoute] Guid id
    )
    {
        return await _service.GetById(id);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id
    )
    {
        var response = new ResponseDTO();
        try
        {
            await _service.DeleteById(id);
            response.IsSuccess = true;
            response.Message = "Success";
            _logger.Log(LogLevel.Information, "User delete {}", id);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = "Exception Occurs : " + ex.Message;
        }

        return Ok(response);
    }
}
