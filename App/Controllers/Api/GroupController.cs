using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestShopApp.App.Filters;
using TestShopApp.App.Models.Group;
using TestShopApp.Common.Data;
using TestShopApp.Common.Repo;
using TestShopApp.Common.Utils;

namespace TestShopApp.App.Controllers.Api;

[ApiController]
[Route("api/v1/groups")]
[RegistrationRequired]
public class GroupController(IUserRepo userRepo, IGroupRepo groupRepo, IMapper mapper) : Controller
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IGroupRepo _groupRepo  = groupRepo;
    private readonly IMapper _mapper = mapper;
    
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] CreateGroupDto groupData)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        AuthUser authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var userRes = await _userRepo.GetUser(authUser.Id);

        if (!userRes.success)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", userRes.message))
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                ContentTypes = { "application/json" }          
            };
        }

        if (userRes.Value.HeadmanOf == null)
        {
            var group = new Group()
            {
                Name = groupData.Name,
                Number = groupData.Number,
                OwnerId = authUser.Id,
                CreatedAt = DateTimeUtils.GetCurrentTimeFormatted(),
                UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted()
            };

            var res = await _groupRepo.CreateGroup(group);

            if (!res.success)
            {
                return new ObjectResult(ApiResponse.Fail().WithField("reason", res.message))
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    ContentTypes = { "application/json" }          
                };
            }
            
            userRes = await _userRepo.MakeHeadmanOf(authUser.Id, groupData.Number);
            
            if (!userRes.success)
            {
                await _groupRepo.DeleteGroup(groupData.Number, authUser.Id);
                
                return new ObjectResult(ApiResponse.Fail().WithField("reason", userRes.message))
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    ContentTypes = { "application/json" }          
                };
            }
            
            return new ObjectResult(ApiResponse.Ok().WithField("redirectTo", "/group"))
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentTypes = { "application/json" }          
            };
        }
        else
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", "User already moderate a group"))
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                ContentTypes = { "application/json" }          
            };
        }
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> Get()
    {
        /*var res = await _groupRepo.GetGroups();

        if (!res.success)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", res.message))
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ContentTypes = { "application/json" }          
            };
        }


        var groups = _mapper.Map<IEnumerable<SafeGroupDto>>(res.Value);
        return new ObjectResult(ApiResponse.Ok().WithField("groups", groups))
        {
            StatusCode = (int)HttpStatusCode.OK,
            ContentTypes = { "application/json" }          
        };*/

        return BadRequest();
    }
    
    [HttpGet]
    [Route("mine")]
    public async Task<IActionResult> Mine()
    {
        AuthUser authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;
        
        var user = await _userRepo.GetUser(authUser.Id);

        if (user.Value.GroupNumber == null)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", "User doesn't have any group"))
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ContentTypes = { "application/json" }          
            };
        }
        
        var group = await _groupRepo.GetGroup(user.Value.GroupNumber);

        if (!group.success)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", group.message))
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ContentTypes = { "application/json" }          
            };
        }

        ;
        return new ObjectResult(ApiResponse.Ok().WithField("group", group.Value))
        {
            StatusCode = (int)HttpStatusCode.OK,
            ContentTypes = { "application/json" }          
        };
    }

    [HttpGet]
    [Route("mine/members")]
    public async Task<IActionResult> Members()
    {
        AuthUser authUser = Request.HttpContext.Items["AuthUser"] as AuthUser;

        var user = await _userRepo.GetUser(authUser.Id);

        if (user.Value.GroupNumber == null)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", "User doesn't have any group"))
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ContentTypes = { "application/json" }
            };
        }

        var group = await _groupRepo.GetGroupMembers(user.Value.GroupNumber);

        if (!group.success)
        {
            return new ObjectResult(ApiResponse.Fail().WithField("reason", group.message))
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ContentTypes = { "application/json" }
            };
        }

        ;
        return new ObjectResult(ApiResponse.Ok().WithField("members", group.Value))
        {
            StatusCode = (int)HttpStatusCode.OK,
            ContentTypes = { "application/json" }
        };
    }
}