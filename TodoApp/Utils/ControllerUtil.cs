using TodoApp.Dto;
using TodoApp.Exceptions;

namespace TodoApp.Utils;

public abstract class ControllerUtil
{
    public static T GetData<T>(HandlerResponse result)
    {
        if (result.Results[0] is not T data)
        {
            throw new EmptyException();
        }

        return data;
    }
}
