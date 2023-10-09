using System.ComponentModel.DataAnnotations;

namespace TodoApp.Commands;

public class DeleteTodoCommand
{
    [Required] public Guid Id { get; set; }
}
