using System.Security.Claims;

namespace CartService.Infrastructure.Middleware
{
    public class TokenLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenLoggingMiddleware> _logger;

        public TokenLoggingMiddleware(RequestDelegate next, ILogger<TokenLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Obtener username desde claims
                var username =
                    context.User.FindFirst("preferred_username")?.Value ??
                    context.User.FindFirst("email")?.Value ??
                    context.User.FindFirst(ClaimTypes.Name)?.Value ??
                    "Unknown";

                var roles = string.Join(",", context.User.Claims
                    .Where(c => c.Type.Contains("role"))
                    .Select(c => c.Value));

                _logger.LogInformation("Token accessed. User={User} Roles={Roles}",
                    username, roles);
            }

            await _next(context);
        }
    }
}
