using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DoAn.Authen
{
    public class AuthenRole: ActionFilterAttribute
    {
        private readonly string _role;
        public AuthenRole(string role)
        {
            _role = role;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var username = session.GetString("Name");
            var role = session.GetInt32("Role").ToString();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectToActionResult("Index", "Account", null);
                return;
            }
            if (!string.Equals(role, _role, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }
        }
    }
}
