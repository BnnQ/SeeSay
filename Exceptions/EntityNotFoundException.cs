namespace SeeSay.Exceptions;

public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(string message = "Entity not found in database.") : base(message)
    {
        //empty
    }
}