using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TestShopApp.App.Filters;
using TestShopApp.App.Models;
using TestShopApp.Common.Data;
using TestShopApp.Common.Repo;
using TestShopApp.Common.Utils;
using TestShopApp.Telegram.Utils;

namespace TestShopApp.App.Controllers.Api;

[ApiController]
[Route("api/v1/users")]
public class UserController(IUserRepo userRepo, IMapper mapper) : ControllerBase
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IMapper _mapper = mapper;
    
    [Route("bake")]
    [HttpPost]
    public async Task<IActionResult> Bake([FromBody] TelegramInitDataRaw data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        (bool isVerified, AuthUser authUser) = TgAuthUtils.VerifyInitData(data.initData);

        if (!isVerified || authUser == null)
        {
            return new ObjectResult(ApiResponse.Fail())
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                ContentTypes = { "application/json" }          
            };
        }
        
        var res = await _userRepo.GetUser(authUser.Id);

        Response.Headers["Authorization"] = "tma " + TgAuthUtils.GenerateExtendedInitData(authUser, res.Value != null);

        return Ok(new
        {
            sucess = true,
            isRegistered = res.Value is not null
        });
    }

    [Route("register")]
    [HttpPost]
    [AuthRequired]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        AuthUser authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;        
        
        var res = await _userRepo.GetUser(authUser.Id);

        if (res.success)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", "user already exists"))
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ContentTypes = { "application/json" }          
            };
        }

        if (data.Role == "student")
        {
            res = await _userRepo.AddUser(new User
            {
                Id = authUser.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Group = data.Group,
                Role = "student",
                CreatedAt = DateTimeUtils.GetCurrentTimeFormatted(),
                UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted()
            });
        }
        else if (data.Role == "teacher")
        {
            res = await _userRepo.AddUser(new User
            {
                Id = authUser.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                ThirdName = data.ThirdName,
                Subject = data.Subject,
                Role = "teacher",
                CreatedAt = DateTimeUtils.GetCurrentTimeFormatted(),
                UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted()
            });
        }
        else
        {
            res = new ExecutionResult<User>(false, null, "Incorrect role");
        }

        if (res.success)
        {
            return new ObjectResult(ApiResponse.Ok().WithField("redirectTo", "/"))
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

    
    [Route("me")]
    [HttpGet]
    [RegistrationRequired]
    public async Task<IActionResult> Me()
    {
        AuthUser authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;
        
        var res = await _userRepo.GetUser(authUser.Id);

        if (!res.success)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", res.message).WithField("redirectTo", "/registration"))
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ContentTypes = { "application/json" }          
            };
        }

        return new ObjectResult(ApiResponse.Ok().WithField("user", res.Value))
        {
            StatusCode = (int)HttpStatusCode.OK,
            ContentTypes = { "application/json" }          
        };
    }

    [Route("update")]
    [HttpPut]
    [RegistrationRequired]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        AuthUser authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var res = await _userRepo.UpdateUser(authUser.Id, user, _mapper);

        if (!res.success)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", res.message))
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                ContentTypes = { "application/json" }
            };
        }

        return new ObjectResult(ApiResponse.Ok())
        {
            StatusCode = (int)HttpStatusCode.OK,
            ContentTypes = { "application/json" }
        };
    }
}

public class TelegramInitDataRaw
{
    [Required]
    public string initData { get; set; }

    [Required]
    public long timestamp { get; set; }
}