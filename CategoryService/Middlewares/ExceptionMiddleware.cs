using CategoryService.Exceptions;
using System.Net;

namespace CategoryService.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await HandleException(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception)
            {
                await HandleException(context, HttpStatusCode.InternalServerError,
                    "Произошла внутренняя ошибка сервера");
            }
        }

        private static Task HandleException(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsJsonAsync(new
            {
                status = (int)statusCode,
                error = statusCode.ToString(),
                message
            });
        }
    }
}
