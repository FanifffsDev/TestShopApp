using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TestShopApp.App.Filters;
using TestShopApp.Common.Data;
using TestShopApp.Common.Repo;
using TestShopApp.Telegram.Utils;

namespace TestShopApp.App.Controllers.Api;

[ApiController]
[Route("api/v1/users")]
public class UserController(ITgUserRepo userRepo) : ControllerBase
{
    private readonly ITgUserRepo _userRepo = userRepo;
    
    [Route("bake")]
    [HttpPost]
    public async Task<IActionResult> Bake([FromBody] TelegramInitDataRaw data)
    {
        (bool isVerified, TgUser user) = TgAuthUtils.VerifyInitData(data.initData);

        if (!isVerified)
        {
            return new ObjectResult(ApiResponse.Fail())
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                ContentTypes = { "application/json" }          
            };
        }

        //await _userRepo.AddUser(user);
        
        Response.Headers["Authorization"] = "tma " + data.initData;
        return Ok(new
        {
            Message = user
        });
    }

    [Route("profile")]
    [HttpGet]
    [AuthRequired]
    public async Task<IActionResult> Profile()
    {
        var profile = await _userRepo.GetUser((Request.HttpContext.Items["User"] as TgUser).Id);
        
        if(profile == null)
            return NotFound();
        
        return new ObjectResult(ApiResponse.Ok().WithField("profile", profile.Value))
        {
            StatusCode = (int)HttpStatusCode.OK,
            ContentTypes = { "application/json" }          
        };
    }
}

public class TelegramInitDataRaw
{
    public string? initData { get; set; }
    public long timestamp { get; set; }
}