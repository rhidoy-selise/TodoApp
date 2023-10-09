using System.ComponentModel.DataAnnotations;
using TodoApp.Models;

namespace TodoApp.Commands;

public class AddTodoCommand
{
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; }
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
