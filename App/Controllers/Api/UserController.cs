using System.Net;
using Microsoft.AspNetCore.Mvc;
using TestShopApp.App.Filters;
using TestShopApp.Common.Data;
using TestShopApp.Common.Repo;
using TestShopApp.Telegram.Utils;

namespace TestShopApp.App.Controllers.Api;

[ApiController]
[Route("api/v1/users")]
public class UserController(IUserRepo userRepo) : ControllerBase
{
    private readonly IUserRepo _userRepo = userRepo;
    
    [Route("bake")]
    [HttpPost]
    public async Task<IActionResult> Bake([FromBody] TelegramInitDataRaw data)
    {
        (bool isVerified, AuthUser user) = TgAuthUtils.VerifyInitData(data.initData);

        if (!isVerified)
        {
            return new ObjectResult(ApiResponse.Fail())
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                ContentTypes = { "application/json" }          
            };
        }
        
        Response.Headers["Authorization"] = "tma " + data.initData;
        return Ok(new
        {
            sucess = true
        });
    }

    [Route("register")]
    [HttpPost]
    [AuthRequired]
    public async Task<IActionResult> Register([FromBody] RegisterData data)
    {
        AuthUser authUser = Request.HttpContext.Items["User"] as AuthUser;
        
        if(authUser == null)
            return BadRequest();
        
        var res = await _userRepo.GetUser(authUser.Id);

        if (res.success)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", "user already exists"))
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ContentTypes = { "application/json" }          
            };
        }

        res = await _userRepo.AddUser(new User
        {
            Id = authUser.Id,
            FirstName = data.firstName,
            LastName = data.lastName,
            ThirdName = data.thirdName,
            Group = data.group,
            Subject = data.subject,
            Role = data.role,
        });

        if (res.success)
        {
            return new ObjectResult(ApiResponse.Ok().WithField("redirectTo", "/home"))
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentTypes = { "application/json" }          
            };
        }
        else
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", res.message))
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ContentTypes = { "application/json" }          
            };
        }
    }
}

public class TelegramInitDataRaw
{
    public string? initData { get; set; }
    public long timestamp { get; set; }
}

public class RegisterData
{
    public string? firstName { get; set; }
    public string? lastName { get; set; }
    public string? thirdName { get; set; }
    public string? group { get; set; }
    public string? subject { get; set; }
    public string? role { get; set; }
}