
namespace NorthWind.Validation.Entities.ValueObjects;

public class ValidationError(string propertyName, string errorMessage)
{
    public string PropertyName => propertyName;
    public string ErrorMessage => errorMessage;
}
