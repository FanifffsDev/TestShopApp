using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestShopApp.Api.Filters;
using TestShopApp.Api.Interefaces;
using TestShopApp.App.Models.Group;
using TestShopApp.Domain.Base;

namespace TestShopApp.Api.Controllers;

[ApiController]
[Route("api/v1/groups")]
[RegistrationRequired]
public class GroupController(IGroupService groupService) : Controller
{
    private readonly IGroupService _groupService = groupService;
    
    [HttpPost]
    [Route("create")]
    public async Task<IResult> Create([FromBody] CreateGroupDto groupData, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        AuthUser? authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var result = await _groupService.CreateGroup(authUser!.Id, groupData, ct);

        return (IResult)result;
    }
    
    [HttpGet]
    [Route("mine")]
    public async Task<IResult> Mine(CancellationToken ct)
    {
        AuthUser? authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var res = await _groupService.GetUserGroup(authUser!.Id, ct);
        return (IResult)res;
    }

    [HttpGet]
    [Route("link")]
    public async Task<IResult> Link(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        AuthUser? authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var result = await _groupService.GetGroupLinkByUserId(authUser!.Id, ct);
        return (IResult)result;
    }

    [HttpGet("invite/{inviteCode}")]
    public async Task<IResult> GetInviteInfo([Required] string inviteCode, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        var result = await _groupService.GetGroupByInviteCode(inviteCode, ct);
        return (IResult)result;
    }

    [HttpPost("join")]
    public async Task<IResult> JoinGroup([FromBody] JoinGroupDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        AuthUser? authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;
      
        var result = await _groupService.JoinGroupByInviteCode(authUser!.Id, dto.InviteCode, ct);
        return (IResult)result;
    }

    [HttpPost("leave")]
    public async Task<IResult> LeaveGroup(CancellationToken ct)
    {
        AuthUser? authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var result = await _groupService.LeaveGroup(authUser!.Id, ct);
        return (IResult)result;
    }

    [HttpPost("remove")]
    public async Task<IResult> Remove([FromBody] long memberId, CancellationToken ct)
    {
        AuthUser? authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var result = await _groupService.RemoveMember(authUser!.Id, memberId, ct);
        return (IResult)result;
    }

    [HttpPost("delete")]
    public async Task<IResult> Delete(CancellationToken ct)
    {
        AuthUser? authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var result = await _groupService.DeleteGroup(authUser!.Id, ct);
        return (IResult)result;
    }

    [HttpPost("grant")]
    public async Task<IResult> Grant([FromBody] long memberId, CancellationToken ct)
    {
        AuthUser? authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var result = await _groupService.TransferOwnership(authUser!.Id, memberId, ct);
        return (IResult)result;
    }
}