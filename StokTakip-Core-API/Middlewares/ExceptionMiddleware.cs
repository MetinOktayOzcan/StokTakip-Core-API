using System.Net;
using System.Text.Json;

namespace StokTakip_Core_API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var exceptionMessage = ex.ToString();
                var maskedMessage = System.Text.RegularExpressions.Regex.Replace(
                    exceptionMessage,
                    @"(?i)(password|şifre|token|secret)([\s=:]+)[^\s;]+",
                    "$1$2***MASKED***"
                );

                _logger.LogError("Beklenmeyen bir sunucu hatası yakalandı. Path: {Path}\nDetay: {MaskedError}", httpContext.Request.Path, maskedMessage);

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                title = "Sunucu Hatası",
                status = context.Response.StatusCode,
                detail = _env.IsDevelopment() ? ex.Message : "İşleminiz gerçekleştirilirken teknik bir hata oluştu.",
                path = context.Request.Path.Value
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}