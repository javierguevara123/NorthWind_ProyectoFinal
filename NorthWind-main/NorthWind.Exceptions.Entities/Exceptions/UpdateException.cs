namespace NorthWind.Exceptions.Entities.Exceptions;

//  La clase permite encapsular las excepciones generadas al persistir los
//  datos en una fuente de datos.
public class UpdateException : Exception
{
    public UpdateException() { }

    public UpdateException(string message)
        : base(message) { }

    public UpdateException(string message, Exception innerException)
        : base(message, innerException) { }

    public UpdateException(Exception exception, IEnumerable<string> entities) : base(exception.Message, exception) => Entities = entities;

    //  Esta propiedad regresa la lista de entidades que causaron la excepción.
    public IEnumerable<string> Entities { get; }
}
