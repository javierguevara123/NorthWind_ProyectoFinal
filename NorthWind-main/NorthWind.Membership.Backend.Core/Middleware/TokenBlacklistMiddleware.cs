using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Backend.Core.Interfaces.Common;

namespace NorthWind.Membership.Backend.Core.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklistService)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                var isBlacklisted = await blacklistService.IsBlacklisted(token);

                if (isBlacklisted)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Token inválido o expirado. Por favor, inicia sesión nuevamente."
                    });
                    return;
                }
            }

            await _next(context);
        }
    }
}
