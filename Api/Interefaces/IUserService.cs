using TestShopApp.App.Models;
using TestShopApp.App.Models.User;
using TestShopApp.App.ReturnTypes;
using TestShopApp.Domain.Base;
using IResult = TestShopApp.App.ReturnTypes.IResult;

namespace TestShopApp.Api.Interefaces
{
    public interface IUserService
    {
        Task<IResult> RegisterUser(long userId, RegisterUserDto user, CancellationToken ct);
        Task<IResult<UserDto?>> GetUser(long userId, CancellationToken ct);
        Task<IResult<UserDto?>> UpdateUserProfile(long userId, UpdateUserProfileDto updateDto, CancellationToken ct);
        Task<IResult<UserDto?>> UpdateUser(UserDto updateDto, CancellationToken ct);
        Task<IResult> BakeUser(TelegramInitDataRawDto data, CancellationToken ct);
        Task<IResult<UserDto?>> UpdateCommonUserData(AuthUser user, CancellationToken ct);
        Task<IResult<UserDto?>> MakeHeadmanOf(long userId, string groupNumber, CancellationToken ct);
    }
}
