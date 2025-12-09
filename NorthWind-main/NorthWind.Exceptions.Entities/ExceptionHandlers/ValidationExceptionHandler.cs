using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthWind.Exceptions.Entities.Exceptions;
using NorthWind.Exceptions.Entities.Extensions;
using NorthWind.Exceptions.Entities.Resources;

namespace NorthWind.Exceptions.Entities.ExceptionHandlers
{
    internal class ValidationExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            bool Handled = false;

            if (exception is ValidationException Ex)
            {

                ProblemDetails Details = new ProblemDetails();
                Details.Status = StatusCodes.Status400BadRequest;
                Details.Type = "https://datatracker.ietf.org/doc/html/rfc7321#section-6.5.1";
                Details.Title = ExceptionMessages.ValidationExceptionTitle;
                Details.Detail = ExceptionMessages.ValidationExceptionDetail;
                Details.Instance = $"{nameof(ProblemDetails)}/{nameof(ValidationException)}";

                Details.Extensions.Add("errors", Ex.Errors.Select(e => new
                {
                    propertyName = e.PropertyName,
                    errorMessage = e.ErrorMessage
                }));

                await httpContext.WriteProblemDetailsAsync(Details);
                Handled = true;

            }

            return Handled;

        }
    }
}
