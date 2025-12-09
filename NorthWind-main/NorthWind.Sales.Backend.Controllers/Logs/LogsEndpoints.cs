using Microsoft.AspNetCore.Builder;

namespace NorthWind.Sales.Backend.Controllers.Logs
{
    public static class LogsEndpoints
    {
        public static WebApplication UseLogsController(this WebApplication app)
        {
            // Este método asegura que el controlador se incluya en el mapeo general.
            // En arquitecturas limpias con controladores explícitos, usamos esto para mantener el orden.
            return app;
        }
    }
}
