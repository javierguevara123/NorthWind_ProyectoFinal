using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Northwind.Sales.WebApi.Extensions;
using NorthWind.Membership.Backend.AspNetIdentity.Options;
using NorthWind.Membership.Backend.AspNetIdentity.Services;
using NorthWind.Membership.Backend.Core.Middleware;
using NorthWind.Membership.Backend.Core.Options;
using NorthWind.Sales.Backend.Controllers.Logs;
using NorthWind.Sales.Backend.DataContexts.EFCore.Options;
using NorthWind.Sales.Backend.IoC;
using NorthWind.Sales.Backend.SmtpGateways.Options;
using NorthWind.Sales.WebApi;
using System.Text;

namespace Northwind.Sales.WebApi;

// Esto expone 2 metodos de extension para configurar los servicios web
// y agregar los middlewares y endpoints de la Web API

internal static class Startup
{
    //  Agregar soporte para documentación Swagger.
    public static WebApplication CreateWebApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGenBearer();

        // --- AGREGAR ESTE BLOQUE ---
        // Esto es OBLIGATORIO para que Swagger encuentre los controladores en otro proyecto
        builder.Services.AddControllers()
               .AddApplicationPart(typeof(LogsController).Assembly);
        // ---------------------------

        builder.Services.AddNorthWindSalesServices(dbObtions =>
            builder.Configuration.GetSection(DBOptions.SectionKey).Bind(dbObtions),
            smtpOptions => builder.Configuration.GetSection(SmtpOptions.SectionKey).Bind(smtpOptions),
            membershipDBOptions => builder.Configuration.GetSection(MembershipDBOptions.SectionKey).Bind(membershipDBOptions),
            jwtOptions => builder.Configuration.GetSection(JwtOptions.SectionKey).Bind(jwtOptions)
        );

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(config =>
            {
                config.AllowAnyMethod();
                config.AllowAnyHeader();
                config.AllowAnyOrigin();
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            builder.Configuration.GetSection(JwtOptions.SectionKey)
            .Bind(options.TokenValidationParameters);
            string SecurityKey = builder.Configuration
            .GetSection(JwtOptions.SectionKey)[nameof(JwtOptions.SecurityKey)];
            byte[] SecurityKeyBytes = Encoding.UTF8.GetBytes(SecurityKey);
            options.TokenValidationParameters.IssuerSigningKey =
            new SymmetricSecurityKey(SecurityKeyBytes);
        });

        builder.Services.AddAuthorization();

        return builder.Build();
    }

    //  Este método se encarga de:
    //  -Habilitar Swagger solo en desarrollo
    //  -Mapear los endpoints de la aplicación
    public static WebApplication ConfigureWebApplication(this WebApplication app)
    {
        app.UseExceptionHandler(builder => { });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Inicializar roles y SuperUser
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                RoleSeeder.SeedRolesAndSuperUser(services).Wait();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error al inicializar roles y SuperUser");
            }
        }

        app.MapNorthWindSalesEndpoints();
        app.UseCors();
        app.UseAuthentication();
        app.UseMiddleware<TokenBlacklistMiddleware>();
        app.UseAuthorization();

        return app;
    }
}
