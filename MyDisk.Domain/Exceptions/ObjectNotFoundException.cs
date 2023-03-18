namespace MyDisk.Domain.Exceptions;

public class ObjectNotFoundException : Exception
{
    public ObjectNotFoundException() : base() { }

    public ObjectNotFoundException(string message) : base(message) { }

    public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException) { }

    public ObjectNotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.") { }
}