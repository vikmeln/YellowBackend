using OrderService.Exceptions;
using System.Net;

namespace OrderService.Middlewares
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
                await Handle(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await Handle(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (ConflictException ex)
            {
                await Handle(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception)
            {
                await Handle(context, HttpStatusCode.InternalServerError, "Ошибка сервера");
            }
        }

        private static Task Handle(HttpContext context, HttpStatusCode code, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsJsonAsync(new
            {
                status = (int)code,
                error = code.ToString(),
                message
            });
        }
    }
}
