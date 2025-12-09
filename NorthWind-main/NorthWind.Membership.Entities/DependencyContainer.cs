using Microsoft.Extensions.DependencyInjection;
using NorthWind.Membership.Entities.Dtos.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserRegistration;
using NorthWind.Membership.Entities.UserLogin;
using NorthWind.Membership.Entities.Validators.UserLogin;
using NorthWind.Membership.Entities.Validators.UserManagement;
using NorthWind.Membership.Entities.Validators.UserRegistration;
using NorthWind.Validation.Entities;

namespace NorthWind.Membership.Entities;

public static class DependencyContainer
{
    public static IServiceCollection AddMembershipValidators(
   this IServiceCollection services)
    {
        services.AddModelValidator<UserRegistrationDto, UserRegistrationDtoValidator>();
        services.AddModelValidator<UpdateUserDto, UpdateUserDtoValidator>();
        services.AddModelValidator<UserCredentialsDto, UserCredentialsDtoValidator>();

        return services;
    }
}

