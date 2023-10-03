namespace TodoApp.Models;

public class Todo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Complete { get; set; }
    public Guid CreateUserGuid { get; set; }
    public Guid AssignedUserGuid { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public TodoStatus Status { get; set; }
}
