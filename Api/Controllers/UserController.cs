using Microsoft.AspNetCore.Mvc;
using TestShopApp.Api.Filters;
using TestShopApp.Api.Interefaces;
using TestShopApp.App.Models;
using TestShopApp.App.Models.User;
using TestShopApp.Domain.Base;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace TestShopApp.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    
    [Route("bake")]
    [HttpPost]
    public async Task<IResult> Bake([FromBody] TelegramInitDataRawDto data, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        var result = await _userService.BakeUser(data, ct);
        return (IResult)result;
    }

    [Route("register")]
    [HttpPost]
    [AuthRequired]
    public async Task<IResult> Register([FromBody] RegisterUserDto user, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        AuthUser authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var result = await _userService.RegisterUser(authUser.Id, user, ct);
        return (IResult)result;
    }

    
    [Route("me")]
    [HttpGet]
    [RegistrationRequired]
    public async Task<IResult> Me(CancellationToken ct)
    {
        AuthUser authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var result = await _userService.GetUser(authUser.Id, ct);
        return (IResult)result;
    }

    [Route("update")]
    [HttpPut]
    [RegistrationRequired]
    public async Task<IResult> Update([FromBody] UpdateUserProfileDto user, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        AuthUser authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var result = await _userService.UpdateUserProfile(authUser.Id, user, ct);
        return (IResult)result;
    }
}