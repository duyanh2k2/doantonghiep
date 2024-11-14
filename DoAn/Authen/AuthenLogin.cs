using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DoAn.Authen
{
    public class AuthenLogin: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var username = session.GetString("Name");

            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToActionResult("Index", "Account", null);
                return;
            }
        }
    }
}
