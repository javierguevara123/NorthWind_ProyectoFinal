using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserManagement;
using NorthWind.Membership.Entities.Responses;

namespace NorthWind.Membership.Backend.Core.UseCases.UserManagement;

public class LogoutInteractor : ILogoutInputPort
{
    private readonly ILogoutOutputPort _outputPort;
    private readonly ITokenBlacklistService _blacklistService;

    public LogoutInteractor(
        ILogoutOutputPort outputPort,
        ITokenBlacklistService blacklistService)
    {
        _outputPort = outputPort;
        _blacklistService = blacklistService;
    }

    public async Task Handle(LogoutDto data)
    {
        var response = new LogoutResponse();

        try
        {
            if (string.IsNullOrWhiteSpace(data.Token))
            {
                response.Success = false;
                response.Message = "Token no proporcionado.";
                response.Errors.Add("El token es requerido para cerrar sesión.");
                _outputPort.Handle(response);
                return;
            }

            // Agregar token a la blacklist con tiempo de expiración fijo (ej: 24 horas)
            // Esto es suficiente porque los tokens JWT tienen expiración propia
            await _blacklistService.AddToBlacklist(data.Token, TimeSpan.FromHours(24));

            response.Success = true;
            response.Message = "Sesión cerrada exitosamente.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al cerrar sesión.";
            response.Errors.Add(ex.Message);
        }

        _outputPort.Handle(response);
    }
}