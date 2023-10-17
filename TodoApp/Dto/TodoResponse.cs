using System.ComponentModel.DataAnnotations;
using TodoApp.Models;

namespace TodoApp.Dto;

public class TodoResponse : ResponseBase
{
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; }
    public bool Complete { get; set; }
    public TodoStatus Status { get; set; }
    public UserGetDto? CreatedUser { get; set; }
    public UserGetDto? AssignedUser { get; set; }

    public static TodoResponse GetDto(Todo todo)
    {
        var dto = new TodoResponse
        {
            Id = todo.Id,
            Name = todo.Name,
            Description = todo.Description,
            Complete = todo.Complete,
            Status = todo.Status,
            CreateDate = todo.CreateDate,
            UpdateDate = todo.UpdateDate
        };
        return dto;
    }
}
