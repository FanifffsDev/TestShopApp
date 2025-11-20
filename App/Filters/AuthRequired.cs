using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using TestShopApp.Common.Data;
using TestShopApp.Telegram.Utils;

namespace TestShopApp.App.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthRequired() : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var headers = context.HttpContext.Request.Headers;

        Console.WriteLine("auth filter has triggered");

        if (!headers.TryGetValue("authorization", out StringValues authHeader))
        {
            context.Result = new RedirectResult("/home");
            return;
        }
        
        string token = authHeader.ToString().Substring("tma ".Length);

        (bool isVerified, AuthUser user) = TgAuthUtils.VerifyInitData(token);

        if (!isVerified || user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["User"] = user;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        return;
    }
}
