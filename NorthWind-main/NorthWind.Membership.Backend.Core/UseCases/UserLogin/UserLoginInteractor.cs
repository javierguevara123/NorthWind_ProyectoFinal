using NorthWind.Membership.Backend.Core.Dtos;
using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserLogin;
using NorthWind.Membership.Backend.Core.Resources;
using NorthWind.Membership.Entities.UserLogin;
using NorthWind.Result.Entities;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;

// ... imports

namespace NorthWind.Membership.Backend.Core.UseCases.UserLogin
{
    internal class UserLoginInteractor(
        IMembershipService membershipService,
        IUserLoginOutputPort presenter,
        IModelValidatorHub<UserCredentialsDto> validationService)
        : IUserLoginInputPort
    {
        public async Task Handle(UserCredentialsDto userData)
        {
            Result<UserDto, IEnumerable<ValidationError>> Result;

            if (!await validationService.Validate(userData))
            {
                Result = new(validationService.Errors);
            }
            else
            {
                // 1. Verificación PREVIA (Para usuarios ya bloqueados desde antes)
                var isLockedOut = await membershipService.IsUserLockedOut(userData.Email);

                if (isLockedOut)
                {
                    Result = new([new ValidationError(nameof(userData.Email), UserLoginMessages.UserAccountLockedErrorMessage)]);
                }
                else
                {
                    // Intentar login
                    var User = await membershipService.GetUserByCredentials(userData);

                    if (User == null)
                    {
                        // 2. VERIFICACIÓN POSTERIOR (CRÍTICO)
                        // Si el login falló, verificamos si ESE fallo provocó el bloqueo inmediato.
                        var isNowLocked = await membershipService.IsUserLockedOut(userData.Email);

                        if (isNowLocked)
                        {
                            // Si acaba de bloquearse, mostramos el mensaje de bloqueo
                            Result = new([new ValidationError(nameof(userData.Email), UserLoginMessages.UserAccountLockedErrorMessage)]);
                        }
                        else
                        {
                            // Si no está bloqueado, es solo error de contraseña
                            Result = new([new ValidationError(nameof(userData.Password), UserLoginMessages.InvalidUserCredentialsErrorMessage)]);
                        }
                    }
                    else
                    {
                        Result = new(User);
                    }
                }
            }

            await presenter.Handle(Result);
        }
    }
}