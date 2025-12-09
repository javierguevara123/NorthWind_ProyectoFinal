using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Exceptions.Entities.Exceptions;

// Permite encapsular la lista de errores de validación en la propiedad: Errors
public class ValidationException : Exception
{
    public ValidationException() { }

    public ValidationException(string message)
        : base(message) { }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException) { }

    public ValidationException(IEnumerable<ValidationError> errors) => Errors = errors;

    public IEnumerable<ValidationError> Errors { get; }
}
