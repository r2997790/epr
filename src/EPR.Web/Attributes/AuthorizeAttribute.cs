using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPR.Web.Attributes;

public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isLoggedIn = context.HttpContext.Session.GetString("IsLoggedIn");
        if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.Path });
        }
    }
}









