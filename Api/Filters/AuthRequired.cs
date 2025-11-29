using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using TestShopApp.Domain.Base;
using TestShopApp.Telegram.Utils;

namespace TestShopApp.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthRequired() : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var headers = context.HttpContext.Request.Headers;

        if (!headers.TryGetValue("authorization", out StringValues authHeader))
        {
            context.Result = new RedirectResult("/home");
            return;
        }
        
        string token = authHeader.ToString().Substring("tma ".Length);

        (bool isVerified, AuthUser? user) = TgAuthUtils.VerifyInitData(token);

        if (!isVerified || user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["AuthUser"] = user;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        return;
    }
}
