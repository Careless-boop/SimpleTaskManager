using System.Text.Json;

namespace SimpleTaskManager.WebApi.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                await HandleExceptionAsync(context);
            }
        }

        private Task HandleExceptionAsync(HttpContext context)
        {
            //return a 500 Internal Server Error response
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = 500,
                Message = "Internal server error occurred."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
