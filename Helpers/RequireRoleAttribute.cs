using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventManagement.Helpers;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public RequireRoleAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var http = context.HttpContext;
        if (!http.IsLoggedIn())
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }
        var currentRole = http.GetRole() ?? string.Empty;
        if (_roles.Length > 0 && !_roles.Any(r => string.Equals(r, currentRole, StringComparison.OrdinalIgnoreCase)))
        {
            context.HttpContext.Session.SetString("TempError", "You do not have permission to perform this action.");
            context.Result = new RedirectToActionResult("List", "Event", null);
        }
    }
}
