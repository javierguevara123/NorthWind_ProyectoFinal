using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NorthWind.Membership.Backend.AspNetIdentity.Entities;
using NorthWind.Membership.Backend.AspNetIdentity.Extensions;
using NorthWind.Membership.Backend.Core.Dtos;
using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Entities.Dtos.Common;
using NorthWind.Membership.Entities.Dtos.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserRegistration;
using NorthWind.Membership.Entities.UserLogin;
using NorthWind.Membership.Entities.Validators;
using NorthWind.Result.Entities;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Membership.Backend.AspNetIdentity.Services
{
    internal class MembershipService(
        UserManager<NorthWindUser> manager,
        RoleManager<IdentityRole> roleManager) : IMembershipService
    {
        public async Task<Result<IEnumerable<ValidationError>>> Register(
            UserRegistrationDto userData,
            string role = "Employee")
        {
            Result<IEnumerable<ValidationError>> Result;

            // Validar que la cédula no esté registrada
            var existingUserByCedula = manager.Users.FirstOrDefault(u => u.Cedula == userData.Cedula);
            if (existingUserByCedula != null)
            {
                return new Result<IEnumerable<ValidationError>>(
                    new[] { new ValidationError("Cedula", "La cédula ya está registrada") });
            }

            var User = new NorthWindUser
            {
                UserName = userData.Email,
                Email = userData.Email,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Cedula = userData.Cedula,
            };

            var CreateResult = await manager.CreateAsync(User, userData.Password);

            if (CreateResult.Succeeded)
            {
                // Aseguramos que si el rol no existe, se intente crear (o fallará si no se corrió el Seeder)
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                await manager.AddToRoleAsync(User, role);
                Result = new Result<IEnumerable<ValidationError>>();
            }
            else
            {
                Result = new Result<IEnumerable<ValidationError>>(
                    CreateResult.Errors.ToValidationErrors());
            }

            return Result;
        }

        public async Task<UserDto> GetUserByCredentials(UserCredentialsDto userData)
        {
            UserDto FoundUser = null;
            var User = await manager.FindByNameAsync(userData.Email);

            if (User != null)
            {
                if (await manager.CheckPasswordAsync(User, userData.Password))
                {
                    await manager.ResetAccessFailedCountAsync(User);
                    var roles = await manager.GetRolesAsync(User);

                    // CAMBIO 2: Pasamos User.Id al DTO para que el JWT lo incluya
                    FoundUser = new UserDto(
                        User.Id,      // <--- NUEVO
                        User.UserName,
                        User.FirstName,
                        User.LastName,
                        User.Cedula,
                        roles);
                }
                else
                {
                    await manager.AccessFailedAsync(User);
                }
            }

            return FoundUser;
        }

        public async Task<bool> IsUserLockedOut(string email)
        {
            var user = await manager.FindByNameAsync(email);
            if (user == null) return false;

            return await manager.IsLockedOutAsync(user);
        }

        public async Task<PagedResultDto<UserInfoDto>> GetAllUsers(int pageNumber, int pageSize)
        {
            // Validar parámetros
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize; // Límite máximo

            var totalCount = await manager.Users.CountAsync();

            var users = await manager.Users
                .OrderBy(u => u.Email)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userInfoList = new List<UserInfoDto>();

            foreach (var user in users)
            {
                var roles = await manager.GetRolesAsync(user);
                var isLockedOut = await manager.IsLockedOutAsync(user);

                userInfoList.Add(new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Cedula = user.Cedula,
                    IsLockedOut = isLockedOut,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles
                });
            }

            return new PagedResultDto<UserInfoDto>
            {
                Items = userInfoList,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<UserInfoDto>> GetLockedOutUsers(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var totalCount = await manager.Users
                .Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow)
                .CountAsync();

            var lockedOutUsers = await manager.Users
                .Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow)
                .OrderBy(u => u.Email)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userInfoList = new List<UserInfoDto>();

            foreach (var user in lockedOutUsers)
            {
                var roles = await manager.GetRolesAsync(user);

                userInfoList.Add(new UserInfoDto
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Cedula = user.Cedula,
                    IsLockedOut = true,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles
                });
            }

            return new PagedResultDto<UserInfoDto>
            {
                Items = userInfoList,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Result<IEnumerable<ValidationError>>> UnlockUser(string email)
        {
            var user = await manager.FindByNameAsync(email);

            if (user == null)
            {
                return new Result<IEnumerable<ValidationError>>(
                    new[] { new ValidationError("Email", "Usuario no encontrado") });
            }

            var result = await manager.SetLockoutEndDateAsync(user, null);
            await manager.ResetAccessFailedCountAsync(user);

            if (result.Succeeded)
            {
                return new Result<IEnumerable<ValidationError>>();
            }

            return new Result<IEnumerable<ValidationError>>(
                result.Errors.ToValidationErrors());
        }

        public async Task<Result<IEnumerable<ValidationError>>> ChangeUserRole(
            string email,
            string newRole)
        {
            var user = await manager.FindByNameAsync(email);

            if (user == null)
            {
                return new Result<IEnumerable<ValidationError>>(
                    new[] { new ValidationError("Email", "Usuario no encontrado") });
            }

            string[] validRoles = { "SuperUser", "Administrator", "Employee" };
            if (!validRoles.Contains(newRole))
            {
                return new Result<IEnumerable<ValidationError>>(
                    new[] { new ValidationError("Role", "Rol inválido") });
            }

            var currentRoles = await manager.GetRolesAsync(user);

            if (currentRoles.Contains("SuperUser") && email == "superuser@northwind.com")
            {
                return new Result<IEnumerable<ValidationError>>(
                    new[] { new ValidationError("Role", "No se puede cambiar el rol del SuperUser principal") });
            }

            var removeResult = await manager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                return new Result<IEnumerable<ValidationError>>(
                    removeResult.Errors.ToValidationErrors());
            }

            var addResult = await manager.AddToRoleAsync(user, newRole);

            if (addResult.Succeeded)
            {
                return new Result<IEnumerable<ValidationError>>();
            }

            return new Result<IEnumerable<ValidationError>>(
                addResult.Errors.ToValidationErrors());
        }

        public async Task InitializeRoles()
        {
            string[] roles = { "SuperUser", "Administrator", "Employee"};

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        // Agregar estos métodos a MembershipService.cs

        // Actualizar en: MembershipService.cs - Método UpdateUser COMPLETO

        public async Task<Result<IEnumerable<ValidationError>>> UpdateUser(
            string email,
            string firstName,
            string lastName,
            string cedula,
            string newPassword,
            string currentUserEmail)
        {
            var userToUpdate = await manager.FindByNameAsync(email);

            if (userToUpdate == null)
            {
                return new Result<IEnumerable<ValidationError>>(
                    new[] { new ValidationError("Email", "Usuario no encontrado") });
            }

            // Validar que la cédula no esté en uso por otro usuario
            if (!string.IsNullOrEmpty(cedula) && cedula != userToUpdate.Cedula)
            {
                var existingUserByCedula = manager.Users.FirstOrDefault(u => u.Cedula == cedula && u.Email != email);
                if (existingUserByCedula != null)
                {
                    return new Result<IEnumerable<ValidationError>>(
                        new[] { new ValidationError("Cedula", "La cédula ya está registrada por otro usuario") });
                }

                // Validar formato de cédula
                if (!CedulaEcuatorianaValidator.IsValid(cedula))
                {
                    return new Result<IEnumerable<ValidationError>>(
                        new[] { new ValidationError("Cedula", "La cédula ecuatoriana no es válida") });
                }
            }

            var currentUser = await manager.FindByNameAsync(currentUserEmail);
            var currentUserRoles = await manager.GetRolesAsync(currentUser);
            var targetUserRoles = await manager.GetRolesAsync(userToUpdate);

            // Verificar permisos
            bool isSuperUser = currentUserRoles.Contains("SuperUser");
            bool isAdmin = currentUserRoles.Contains("Administrator");
            bool isOwnProfile = email == currentUserEmail;

            // Reglas de autorización
            if (!isSuperUser)
            {
                if (!isOwnProfile && !isAdmin)
                {
                    return new Result<IEnumerable<ValidationError>>(
                        new[] { new ValidationError("Authorization", "No tiene permisos para modificar este usuario") });
                }

                if (isAdmin && !isOwnProfile && !targetUserRoles.Contains("Employee"))
                {
                    return new Result<IEnumerable<ValidationError>>(
                        new[] { new ValidationError("Authorization", "Los administradores solo pueden modificar empleados") });
                }
            }

            // Actualizar datos
            userToUpdate.FirstName = firstName;
            userToUpdate.LastName = lastName;
            userToUpdate.Cedula = cedula;

            var updateResult = await manager.UpdateAsync(userToUpdate);

            if (!updateResult.Succeeded)
            {
                return new Result<IEnumerable<ValidationError>>(
                    updateResult.Errors.ToValidationErrors());
            }

            // Cambiar contraseña si se proporciona
            if (!string.IsNullOrEmpty(newPassword))
            {
                // Validar requisitos de contraseña manualmente
                var passwordErrors = new List<ValidationError>();

                if (newPassword.Length < 6)
                    passwordErrors.Add(new ValidationError("NewPassword", "La contraseña debe tener al menos 6 caracteres"));

                if (!newPassword.Any(c => char.IsLower(c)))
                    passwordErrors.Add(new ValidationError("NewPassword", "La contraseña debe contener al menos una letra minúscula"));

                if (!newPassword.Any(c => char.IsUpper(c)))
                    passwordErrors.Add(new ValidationError("NewPassword", "La contraseña debe contener al menos una letra mayúscula"));

                if (!newPassword.Any(c => char.IsDigit(c)))
                    passwordErrors.Add(new ValidationError("NewPassword", "La contraseña debe contener al menos un dígito"));

                if (!newPassword.Any(c => !char.IsLetterOrDigit(c)))
                    passwordErrors.Add(new ValidationError("NewPassword", "La contraseña debe contener al menos un carácter no alfanumérico"));

                if (passwordErrors.Any())
                {
                    return new Result<IEnumerable<ValidationError>>(passwordErrors);
                }

                var token = await manager.GeneratePasswordResetTokenAsync(userToUpdate);
                var passwordResult = await manager.ResetPasswordAsync(userToUpdate, token, newPassword);

                if (!passwordResult.Succeeded)
                {
                    return new Result<IEnumerable<ValidationError>>(
                        passwordResult.Errors.ToValidationErrors());
                }
            }

            return new Result<IEnumerable<ValidationError>>();
        }

        public async Task<Result<IEnumerable<ValidationError>>> DeleteUser(
            string email,
            string currentUserEmail)
        {
            var userToDelete = await manager.FindByNameAsync(email);

            if (userToDelete == null)
            {
                return new Result<IEnumerable<ValidationError>>(
                    new[] { new ValidationError("Email", "Usuario no encontrado") });
            }

            // No permitir que el SuperUser se elimine a sí mismo
            if (email == "superuser@northwind.com")
            {
                return new Result<IEnumerable<ValidationError>>(
                    new[] { new ValidationError("Authorization", "No se puede eliminar al SuperUser principal") });
            }

            var currentUser = await manager.FindByNameAsync(currentUserEmail);
            var currentUserRoles = await manager.GetRolesAsync(currentUser);
            var targetUserRoles = await manager.GetRolesAsync(userToDelete);

            // Verificar permisos
            bool isSuperUser = currentUserRoles.Contains("SuperUser");
            bool isAdmin = currentUserRoles.Contains("Administrator");
            bool isOwnProfile = email == currentUserEmail;

            // Reglas de autorización
            if (!isSuperUser)
            {
                if (!isOwnProfile && !isAdmin)
                {
                    return new Result<IEnumerable<ValidationError>>(
                        new[] { new ValidationError("Authorization", "No tiene permisos para eliminar este usuario") });
                }

                if (isAdmin && !targetUserRoles.Contains("Employee"))
                {
                    return new Result<IEnumerable<ValidationError>>(
                        new[] { new ValidationError("Authorization", "Los administradores solo pueden eliminar empleados") });
                }
            }

            var deleteResult = await manager.DeleteAsync(userToDelete);

            if (deleteResult.Succeeded)
            {
                return new Result<IEnumerable<ValidationError>>();
            }

            return new Result<IEnumerable<ValidationError>>(
                deleteResult.Errors.ToValidationErrors());
        }

        // Agregar al final de MembershipService.cs

        public async Task<UserInfoDto> GetUserById(string userId)
        {
            var user = await manager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var roles = await manager.GetRolesAsync(user);
            var isLockedOut = await manager.IsLockedOutAsync(user);

            return new UserInfoDto
            {
                Id = user.Id,  // ← Asegúrate de que UserInfoDto tenga esta propiedad
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Cedula = user.Cedula,
                IsLockedOut = isLockedOut,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                Roles = roles
            };
        }

        public async Task<UserInfoDto> GetUserByEmail(string email)
        {
            var user = await manager.FindByNameAsync(email);

            if (user == null)
            {
                return null;
            }

            var roles = await manager.GetRolesAsync(user);
            var isLockedOut = await manager.IsLockedOutAsync(user);

            return new UserInfoDto
            {
                Id = user.Id,  // ← Asegúrate de que UserInfoDto tenga esta propiedad
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Cedula = user.Cedula,
                IsLockedOut = isLockedOut,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                Roles = roles
            };
        }
    }
}