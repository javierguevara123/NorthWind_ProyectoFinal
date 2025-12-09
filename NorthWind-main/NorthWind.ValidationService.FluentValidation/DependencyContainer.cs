using Microsoft.Extensions.DependencyInjection;
using NorthWind.Validation.Entities.Interfaces;

namespace NorthWind.ValidationService.FluentValidation;

public static class DependencyContainer
{
    public static IServiceCollection AddValidationService(
   this IServiceCollection services)
    {
        services.AddScoped(typeof(IValidationService<>),
        typeof(FluentValidationService<>));
        return services;
    }
}

