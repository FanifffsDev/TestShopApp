using TestShopApp.App.Models.Group;
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


        Task<IResult> DeleteGroup(long headmanId, CancellationToken ct);

        //Task<IResult> Update(long callerId, UpdateGroup createGroupDto, CancellationToken ct);
        //TODO Дать возможность старосте обновлять информацию о группе т е название.
        //Возможно дать разные приколы для кастомизации, чтобы дать кастомную роль старосте, участникам группы и тд.
        //Изменить визуал группы, т е цвета, фон и т д.


        Task<IResult> TransferOwnership(long headmanId, long newHeadmanId, CancellationToken ct);
    }
}
