using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NorthWind.Membership.Backend.AspNetIdentity.Entities;

namespace NorthWind.Membership.Backend.AspNetIdentity.Services
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAndSuperUser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<NorthWindUser>>();

            // CAMBIO: Roles del sistema
            string[] roles = { "SuperUser", "Administrator", "Employee", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Crear SuperUser si no existe
            var superUserEmail = "superuser@northwind.com";
            var superUser = await userManager.FindByEmailAsync(superUserEmail);

            if (superUser == null)
            {
                // --- NUEVO: Lógica para cargar foto por defecto ---
                byte[]? defaultProfilePicture = null;

                // Define la ruta donde guardas la imagen por defecto. 
                // Asegúrate de que el archivo 'default-avatar.png' exista y se copie al directorio de salida (Copy to Output Directory).
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "default-avatar.png");

                try
                {
                    if (File.Exists(imagePath))
                    {
                        defaultProfilePicture = await File.ReadAllBytesAsync(imagePath);
                    }
                }
                catch
                {
                    // Si falla la lectura, simplemente se crea sin foto para no detener el despliegue
                    defaultProfilePicture = null;
                }
                // ------------------------------------------------

                superUser = new NorthWindUser
                {
                    UserName = superUserEmail,
                    Email = superUserEmail,
                    FirstName = "Super",
                    LastName = "User",
                    Cedula = "1234567890",
                    EmailConfirmed = true,
                    // ASIGNACIÓN DE LA FOTO
                };

                var result = await userManager.CreateAsync(superUser, "SuperUser123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superUser, "SuperUser");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(superUser, "SuperUser"))
                {
                    await userManager.AddToRoleAsync(superUser, "SuperUser");
                }
            }
        }
    }
}