using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PlexSSO.Attributes
{
    /// <summary>
    /// Simple attribute to gate admin routes using session-based auth.
    /// </summary>
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpCtx = context.HttpContext;
            var isAdmin = httpCtx.Session.GetInt32("admin-id") != null;

            var path = httpCtx.Request.Path.Value ?? "";

            // Allow /admin/login without restriction
            if (path.StartsWith("/admin/login", StringComparison.OrdinalIgnoreCase))
                return;

            if (!isAdmin)
            {
                context.Result = new RedirectResult("/admin/login");
            }
        }
    }
}
