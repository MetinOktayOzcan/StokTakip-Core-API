using System.Net;

namespace StokTakip_Core_API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                statusCode = context.Response.StatusCode,
                mesaj = "Sistemde geçici bir teknik aksaklık yaşanmaktadır. İşleminiz şu anda gerçekleştirilemiyor.",
                hataDetayi = exception.Message
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}