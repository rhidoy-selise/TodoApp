using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TodoApp.Models;

namespace TodoApp.Dto;

public class TodoBaseDto
{
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; }
}

public class TodoCreateDto : TodoBaseDto
{
    [Required] public Guid CreatedUserGuid { get; set; }
    [Required] public Guid AssignedUserGuid { get; set; }

    public Todo GetTodo()
    {
        return new Todo
        {
            Id = Guid.NewGuid(),
            Name = Name,
            Description = Description,
            CreateDate = DateTime.Now,
            CreateUserGuid = CreatedUserGuid,
            AssignedUserGuid = AssignedUserGuid,
            Complete = false,
            Status = TodoStatus.Pending
        };
    }
}

public class TodoGetDto : TodoBaseDto
{
    public Guid Id { get; set; }
    public bool Complete { get; set; }
    public TodoStatus Status { get; set; }
    public UserGetDto CreatedUser { get; set; }
    public UserGetDto AssignedUser { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}

public class TodoUpdateDto : TodoBaseDto
{
    [JsonIgnore] public Guid Id { get; set; }
    public bool Complete { get; set; }
    public TodoStatus Status { get; set; }
}
