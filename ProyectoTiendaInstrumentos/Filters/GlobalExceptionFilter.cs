using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ProyectoTiendaInstrumentos.Filters;

public sealed class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> logger;
    private readonly IWebHostEnvironment env;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment env)
    {
        this.logger = logger;
        this.env = env;
    }

    public void OnException(ExceptionContext context)
    {
        // Centralizamos el control de errores para no llenar los controladores de try/catch.
        logger.LogError(context.Exception, "Unhandled exception executing {Action}", context.ActionDescriptor.DisplayName);

        // Si es una petición AJAX/JSON devolvemos un error estándar.
        var accept = context.HttpContext.Request.Headers.Accept.ToString();
        if (accept.Contains("application/json", StringComparison.OrdinalIgnoreCase)
            || string.Equals(context.HttpContext.Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new JsonResult(new
            {
                error = "Ocurrió un error inesperado.",
                detail = env.IsDevelopment() ? context.Exception.Message : null
            })
            { StatusCode = StatusCodes.Status500InternalServerError };

            context.ExceptionHandled = true;
            return;
        }

        // Para vistas, redirigimos a la pantalla de error.
        context.Result = new RedirectToActionResult("Error", "Home", null);
        context.ExceptionHandled = true;
    }
}
