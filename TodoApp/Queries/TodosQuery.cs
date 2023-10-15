using TodoApp.Utils;

namespace TodoApp.Queries;

public class TodosQuery : Paging
{
    public Guid? Id { get; set; }
}
