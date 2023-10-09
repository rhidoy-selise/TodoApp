using System.ComponentModel.DataAnnotations;
using TodoApp.Models;

namespace TodoApp.Commands;

public class UpdateTodoCommand
{
    [Required] public Guid Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; }
    public bool Complete { get; set; }
    public TodoStatus Status { get; set; }

    public void UpdateTodo(Todo todo)
    {
        todo.Name = Name;
        todo.Description = Description;
        todo.Complete = Complete;
        todo.Status = Status;
        todo.UpdateDate = DateTime.Now;
    }
}
