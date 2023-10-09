using System.ComponentModel.DataAnnotations;

namespace TodoApp.Queries;

public class GetTodoByIdQuery
{
    [Required]
    public Guid Id { get; set; }
}
