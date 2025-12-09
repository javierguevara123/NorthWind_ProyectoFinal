using Microsoft.Extensions.DependencyInjection;
using NorthWind.Entities.Interfaces;
using NorthWind.Sales.Backend.SmtpGateways.Options;

namespace NorthWind.Sales.Backend.SmtpGateways
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddMailServices(
       this IServiceCollection services,
       Action<SmtpOptions> configureSmtpOptions)
        {
            services.AddSingleton<IMailService, MailService>();
            services.Configure(configureSmtpOptions);
            return services;
        }
    }

}
