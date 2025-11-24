using BK.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BK.Attributes
{
    public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRolesAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedObjectResult(new ErrorResponse
                {
                    Error = "Не авторизован",
                    Details = "Требуется авторизация"
                });
                return;
            }

            if (_roles.Length > 0 && !_roles.Any(role => user.IsInRole(role)))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}