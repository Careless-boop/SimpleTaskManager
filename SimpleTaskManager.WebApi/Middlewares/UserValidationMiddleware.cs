using Microsoft.AspNetCore.Authorization;
using SimpleTaskManager.BLL.Interfaces;

namespace SimpleTaskManager.WebApi.Middlewares
{
    //middleware to validate user and store him in HttpContext
    public class UserValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            var endpoint = context.GetEndpoint();
            var isAuthorized = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;

            if (isAuthorized)
            {
                // getting scoped service
                var jwtTokensService = serviceProvider.GetRequiredService<IJwtTokensService>();

                var authorizationHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    var token = authorizationHeader.Replace("Bearer ", "");
                    var user = await jwtTokensService.GetUserFromAccessTokenAsync(token);

                    if (user != null)
                    {
                        context.Items["User"] = user;
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Invalid token.");
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Authorization token is missing.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
