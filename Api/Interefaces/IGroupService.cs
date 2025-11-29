using TestShopApp.App.Models.Group;
using TestShopApp.App.ReturnTypes;
using TestShopApp.Domain.Base;
using IResult = TestShopApp.App.ReturnTypes.IResult;

namespace TestShopApp.Api.Interefaces
{
    public interface IGroupService
    {
        Task<IResult> CreateGroup(long callerId, CreateGroupDto createGroupDto, CancellationToken ct);
        Task<IResult> GetUserGroup(long userId, CancellationToken ct);
        Task<IResult> GetGroupLinkByUserId(long userId, CancellationToken ct);
        Task<IResult> GetGroupByInviteCode(string inviteCode, CancellationToken ct);

        Task<IResult> JoinGroupByInviteCode(long userId, string inviteCode, CancellationToken ct);

        Task<IResult> LeaveGroup(long userId, CancellationToken ct);

        Task<IResult> RemoveMember(long headmanId, long memberId, CancellationToken ct);


        //Task<IResult> DeleteGroup(long callerId, string groupNumber, CancellationToken ct);
        //Task<IResult> Update(long callerId, UpdateGroup createGroupDto, CancellationToken ct);
    }
}
