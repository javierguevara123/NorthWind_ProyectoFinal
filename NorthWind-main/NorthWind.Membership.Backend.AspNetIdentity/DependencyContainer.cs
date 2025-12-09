using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NorthWind.Membership.Backend.AspNetIdentity.DataContexts;
using NorthWind.Membership.Backend.AspNetIdentity.Entities;
using NorthWind.Membership.Backend.AspNetIdentity.Options;
using NorthWind.Membership.Backend.AspNetIdentity.Services;
using NorthWind.Membership.Backend.Core.Interfaces.Common;

namespace NorthWind.Membership.Backend.AspNetIdentity
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddMembershipIdentityServices(
            this IServiceCollection services,
            Action<MembershipDBOptions> configureMembershipDBOptions)
        {
            services.AddDbContext<NorthWindMembershipContext>();

            // Configurar Identity con opciones de bloqueo
            services.AddIdentityCore<NorthWindUser>(options =>
            {
                // ========== Configuración de bloqueo de cuenta ==========
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                // ========== Configuración de contraseñas ==========
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6; // ← Cambiado a 6 para ser consistente con tu validación

                // ========== Configuración de usuario ==========
                options.User.RequireUniqueEmail = true; // ← RECOMENDADO: Email único
            })
            .AddRoles<IdentityRole>() // ← Simplificado
            .AddEntityFrameworkStores<NorthWindMembershipContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IMembershipService, MembershipService>();
            services.Configure(configureMembershipDBOptions);

            return services;
        }
    }
}