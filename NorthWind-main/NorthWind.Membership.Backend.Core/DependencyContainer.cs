using Microsoft.Extensions.DependencyInjection;
using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserLogin;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Backend.Core.Interfaces.UserRegistration;
using NorthWind.Membership.Backend.Core.Options;
using NorthWind.Membership.Backend.Core.Presenters.UserLogin;
using NorthWind.Membership.Backend.Core.Presenters.UserManagement;
using NorthWind.Membership.Backend.Core.Presenters.UserRegistration;
using NorthWind.Membership.Backend.Core.Services;
using NorthWind.Membership.Backend.Core.UseCases.UserLogin;
using NorthWind.Membership.Backend.Core.UseCases.UserManagement;
using NorthWind.Membership.Backend.Core.UseCases.UserRegistration;
using NorthWind.Membership.Entities;
using NorthWind.Validation.Entities;

namespace NorthWind.Membership.Backend.Core;

public static class DependencyContainer
{
    public static IServiceCollection AddMembershipCoreServices(
        this IServiceCollection services,
        Action<JwtOptions> configureJwtOptions)
    {
        services.AddMembershipValidators();
        services.AddDefaultModelValidatorHub();

        // ========== Token Blacklist Service ==========
        services.AddDistributedMemoryCache(); // Para desarrollo - usa Redis en producción
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

        // ========== User Registration ==========
        services.AddScoped<IUserRegistrationInputPort, UserRegistrationInteractor>();
        services.AddScoped<IUserRegistrationOutputPort, UserRegistrationPresenter>();

        // ========== User Login ==========
        services.AddScoped<IUserLoginInputPort, UserLoginInteractor>();
        services.AddScoped<IUserLoginOutputPort, UserLoginPresenter>();

        // ========== User Logout ==========
        services.AddScoped<ILogoutInputPort, LogoutInteractor>();
        services.AddScoped<ILogoutOutputPort, LogoutPresenter>();

        // ========== User Management - Get All Users ==========
        services.AddScoped<IGetAllUsersInputPort, GetAllUsersInteractor>();
        services.AddScoped<IGetAllUsersOutputPort, GetAllUsersPresenter>();

        // ========== User Management - Get User By Id ==========
        services.AddScoped<IGetUserByIdInputPort, GetUserByIdInteractor>();
        services.AddScoped<IGetUserByIdOutputPort, GetUserByIdPresenter>();

        // ========== User Management - Get Locked Out Users ==========
        services.AddScoped<IGetLockedOutUsersInputPort, GetLockedOutUsersInteractor>();
        services.AddScoped<IGetLockedOutUsersOutputPort, GetLockedOutUsersPresenter>();

        // ========== User Management - Unlock User ==========
        services.AddScoped<IUnlockUserInputPort, UnlockUserInteractor>();
        services.AddScoped<IUnlockUserOutputPort, UnlockUserPresenter>();

        // ========== User Management - Change User Role ==========
        services.AddScoped<IChangeUserRoleInputPort, ChangeUserRoleInteractor>();
        services.AddScoped<IChangeUserRoleOutputPort, ChangeUserRolePresenter>();

        // ========== User Management - Update User ==========
        services.AddScoped<IUpdateUserInputPort, UpdateUserInteractor>();
        services.AddScoped<IUpdateUserOutputPort, UpdateUserPresenter>();

        // ========== User Management - Delete User ==========
        services.AddScoped<IDeleteUserInputPort, DeleteUserInteractor>();
        services.AddScoped<IDeleteUserOutputPort, DeleteUserPresenter>();

        // ========== JWT Service ==========
        services.AddSingleton<JwtService>();
        services.Configure(configureJwtOptions);

        return services;
    }
}
