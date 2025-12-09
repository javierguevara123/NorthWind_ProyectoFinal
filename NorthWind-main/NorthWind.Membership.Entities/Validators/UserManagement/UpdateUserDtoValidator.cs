using NorthWind.Membership.Entities.Dtos.UserManagement;
using NorthWind.Membership.Entities.Resources;
using NorthWind.Validation.Entities.Abstractions;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.Membership.Entities.Validators.UserManagement;

internal class UpdateUserDtoValidator :
    AbstractModelValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator(
        IValidationService<UpdateUserDto> validationService) :
        base(validationService)
    {
        AddRuleFor(u => u.Email)
            .NotEmpty(UserRegistrationMessages.RequiredEmailErrorMessage)
            .EmailAddress(UserRegistrationMessages.InvalidEmailErrorMessage);

        AddRuleFor(u => u.FirstName)
            .NotEmpty(UserRegistrationMessages.RequiredFirstNameErrorMessage);

        AddRuleFor(u => u.LastName)
            .NotEmpty(UserRegistrationMessages.RequiredLastNameErrorMessage);

        // Validación de Cédula
        AddRuleFor(u => u.Cedula)
            .NotEmpty(UserRegistrationMessages.RequiredCedulaErrorMessage)
            .Length(10, UserRegistrationMessages.InvalidCedulaLengthErrorMessage)
            .Must(CedulaEcuatorianaValidator.IsValid,
                UserRegistrationMessages.InvalidCedulaErrorMessage);

        // NewPassword es OPCIONAL - No validamos si está vacío
        // La validación de contraseña se hará en el servicio si se proporciona
    }
}