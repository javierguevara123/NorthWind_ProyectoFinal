using Microsoft.Extensions.DependencyInjection;
using NorthWind.DomainLogs.Entities.Interfaces;
using NorthWind.DomainLogs.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWind.DomainLogs.Entities
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddDomainLogsServices(
       this IServiceCollection services)
        {
            services.AddScoped<IDomainLogger, DomainLogger>();
            return services;
        }
    }

}
