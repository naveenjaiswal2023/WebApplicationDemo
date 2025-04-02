using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace WebApplication1.Filters
{
    public class RoleBasedAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string _requiredRole;
        private readonly ILogger<RoleBasedAuthorizationFilter> _logger;

        public RoleBasedAuthorizationFilter(string requiredRole, ILogger<RoleBasedAuthorizationFilter> logger)
        {
            _requiredRole = requiredRole;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogWarning("User is not authenticated.");
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            if (!roleClaims.Contains(_requiredRole))
            {
                _logger.LogWarning($"User does not have the required role: {_requiredRole}");
                context.Result = new ForbidResult();
            }
        }
    }
}
