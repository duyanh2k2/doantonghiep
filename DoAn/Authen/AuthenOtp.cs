using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.Authen
{
    public class AuthenOtp : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var username = session.GetString("OTP_Verified");

            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToActionResult("Otp", "Account", null);
                return;
            }
        }
    }
}
