namespace TodoApp.Models;

public class Todo : EntityBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Complete { get; set; }
    public Guid CreateUserGuid { get; set; }
    public Guid AssignedUserGuid { get; set; }
    public TodoStatus Status { get; set; }
}
