using System.Runtime.CompilerServices;

namespace TodoListApi.Middleware
{
    public class ForbidNonActiveMiddleware
    {


        private readonly RequestDelegate _next;

        public ForbidNonActiveMiddleware(RequestDelegate requestDelegate)
        {
            _next = requestDelegate;
        }


        public async Task InvokeAsync(HttpContext context)
        {

            bool isUserAuthenticated = context.User.Identity?.IsAuthenticated == true;
            if (isUserAuthenticated)
            {
                bool isUserActive = context.User.FindFirst("IsActive")?.Value == "True";

                if (!isUserActive)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("YOU ARE BANNED, CONTACT ADMINS IF YOU THINK IT'S MISTAKE...");
                    return;
                }
            }


            await _next(context);
        }

    }
}
