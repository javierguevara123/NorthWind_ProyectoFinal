using Microsoft.Extensions.DependencyInjection;
using NorthWind.Transactions.Entities.Interfaces;
using NorthWind.Transactions.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWind.Transactions.Entities
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddTransactionServices(
       this IServiceCollection services)
        {
            services.AddTransient<IDomainTransaction, DomainTransaction>();
            return services;
        }
    }
}
