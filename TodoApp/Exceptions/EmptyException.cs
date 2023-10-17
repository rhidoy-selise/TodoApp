namespace TodoApp.Exceptions;

public class EmptyException : Exception
{
    public EmptyException() : this("Empty result found")
    {
    }

    public EmptyException(string message) : base(message)
    {
    }

    public EmptyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
