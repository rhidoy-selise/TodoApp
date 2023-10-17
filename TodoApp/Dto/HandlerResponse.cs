namespace TodoApp.Dto;

public class HandlerResponse
{
    public HandlerResponse(object results)
    {
        results ??= new List<object>();
        Results = results;
    }

    public dynamic Results { get; set; }
}
