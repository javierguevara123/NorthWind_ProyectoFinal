using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace NorthWind.Exceptions.Entities.Extensions;

internal static class HttpContextExtensions
{
    public static async ValueTask WriteProblemDetailsAsync(
        this HttpContext context,
        ProblemDetails details
    )
    {
        //  La variable ProblemDetails: tiene su propio content-type
        context.Response.ContentType = "application/problem+json";

        //  Establecer el código de respuesta HTTP que se va a regresar
        context.Response.StatusCode = details.Status.Value;

        //  Serializar la respuesta
        var Stream = context.Response.Body;

        await JsonSerializer.SerializeAsync(Stream, details);
    }
}
