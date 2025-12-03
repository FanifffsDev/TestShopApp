using AutoMapper;
using AutoMapper.Execution;
using Telegram.Bot.Types;
using TestShopApp.Api.Interefaces;
using TestShopApp.App.Interfaces;
using TestShopApp.App.Models.Group;
using TestShopApp.App.ReturnTypes;
using TestShopApp.App.Utils;
using TestShopApp.Domain.Base;
using static TestShopApp.App.ReturnTypes.ResultImport;
using IResult = TestShopApp.App.ReturnTypes.IResult;

namespace TestShopApp.App.Services
{
    public class GroupService(ILogger<GroupService> logger,
        IGroupRepo groupRepo,
        IUserRepo userRepo,
        IUserService userService, IMapper mapper) : IGroupService
    {
        private readonly ILogger<GroupService> _logger = logger;
        private readonly IUserService _userService = userService;
        private readonly IGroupRepo _groupRepo = groupRepo;
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IMapper _mapper = mapper;

        private readonly string _botUsername = Environment.GetEnvironmentVariable("BOT_USERNAME")
                ?? throw new InvalidOperationException("BOT_USERNAME environment variable is not set");

        public async Task<IResult> CreateGroup(long callerId, CreateGroupDto createGroupDto, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Attempting to create group {GroupNumber} by user {CallerId}",
                    createGroupDto.Number, callerId);

                var userResult = await _userService.GetUser(callerId, ct);

                if (userResult.IsFailure)
                {
                    _logger.LogWarning("User {CallerId} not found when creating group", callerId);
                    return userResult;
                }

                if (!string.IsNullOrEmpty(userResult.Value!.HeadmanOf) ||
                    !string.IsNullOrEmpty(userResult.Value!.GroupNumber))
                {
                    _logger.LogWarning("User {CallerId} already moderates or participates in a group", callerId);
                    return AlreadyExists("User already moderates a group or participates in one");
                }

                var existingGroup = await _groupRepo.GetGroup(createGroupDto.Number, ct);
                if (existingGroup != null)
                {
                    _logger.LogWarning("Group with number {GroupNumber} already exists", createGroupDto.Number);
                    return AlreadyExists($"Group with number {createGroupDto.Number} already exists");
                }

                var group = new Group
                {
                    Name = createGroupDto.Name,
                    Number = createGroupDto.Number,
                    CreatedAt = DateTimeUtils.GetCurrentTimeFormatted(),
                    UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted()
                };

                await _groupRepo.CreateGroup(group, ct);
                var groupSaved = await _groupRepo.SaveChangesAsync(ct);

                if (!groupSaved)
                {
                    _logger.LogError("Failed to save group {GroupNumber} to database", createGroupDto.Number);
                    return Internal("Failed to create group");
                }

                var headmanResult = await _userService.MakeHeadmanOf(callerId, createGroupDto.Number, ct);

                if (headmanResult.IsFailure)
                {
                    _logger.LogError("Failed to make user {CallerId} headman of group {GroupNumber}. Rolling back group creation",
                        callerId, createGroupDto.Number);

                    _groupRepo.DeleteGroup(group);
                    await _groupRepo.SaveChangesAsync(ct);

                    return headmanResult;
                }

                _logger.LogInformation("Group {GroupNumber} created successfully by user {CallerId}",
                    createGroupDto.Number, callerId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating group {GroupNumber} by user {CallerId}",
                    createGroupDto?.Number ?? "unknown", callerId);
                return Internal($"Error while creating group: {ex.Message}");
            }
        }

        public async Task<IResult> GetUserGroup(long userId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Fetching group information for user {UserId}", userId);

                var userResult = await _userService.GetUser(userId, ct);
                if (userResult.IsFailure)
                {
                    _logger.LogWarning("Failed to get user {UserId}: {Error}", userId, userResult.Message);
                    return userResult;
                }

                var user = userResult.Value;

                if (string.IsNullOrEmpty(user!.GroupNumber))
                {
                    _logger.LogWarning("User {UserId} is not a member of any group", userId);
                    return NotFound("User is not a member of any group");
                }

                var group = await _groupRepo.GetGroup(user.GroupNumber, ct);
                if (group == null)
                {
                    _logger.LogError("Group {GroupNumber} not found for user {UserId}", user.GroupNumber, userId);
                    return NotFound($"Group {user.GroupNumber} not found");
                }

                var groupMembers = await _userRepo.GetGroupMembers(user.GroupNumber, ct);
                if (groupMembers == null || !groupMembers.Any())
                {
                    _logger.LogWarning("No members found for group {GroupNumber}", user.GroupNumber);
                    return NotFound("No members found for group");
                }

                var membersDto = _mapper.Map<IEnumerable<GroupMemberDto>>(groupMembers);

                bool isHeadman = !string.IsNullOrEmpty(user.HeadmanOf) &&
                                       user.HeadmanOf == user.GroupNumber;

                _logger.LogInformation("Successfully fetched group {GroupNumber} information for user {UserId}",
                    user.GroupNumber, userId);

                return Ok(group).WithField("group", _mapper.Map<SafeGroupDto>(group))
                    .WithField("members", membersDto).WithFieldIf("isHeadman", isHeadman, isHeadman);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching group information for user {UserId}", userId);
                return Internal($"Error while fetching group information: {ex.Message}");
            }
        }

        public async Task<IResult> GetGroupLinkByUserId(long userId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Generating invite link for user {UserId}", userId);

                var userResult = await _userService.GetUser(userId, ct);
                if (userResult.IsFailure)
                {
                    _logger.LogWarning("Failed to get user {UserId}: {Error}", userId, userResult.Message);
                    return userResult;
                }

                var user = userResult.Value;

                if (string.IsNullOrEmpty(user!.HeadmanOf))
                {
                    _logger.LogWarning("User {UserId} is not a headman of any group", userId);
                    return Unauthorized("User doesn't moderate any group");
                }

                var group = await _groupRepo.GetGroup(user.HeadmanOf, ct);
                if (group == null)
                {
                    _logger.LogError("Group {GroupNumber} not found for headman {UserId}", user.HeadmanOf, userId);
                    return NotFound($"Group {user.HeadmanOf} not found");
                }

                var inviteCode = GroupLinkGenerator.GenerateInviteCode(userId, user.HeadmanOf);
                var inviteLink = GroupLinkGenerator.GenerateInviteLink(userId, user.HeadmanOf, _botUsername);

                _logger.LogInformation("Successfully generated invite link for group {GroupNumber} by user {UserId}",
                    user.HeadmanOf, userId);

                return Ok(group).WithField("inviteLink", inviteLink)
                    .WithField("inviteCode", inviteCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating invite link for user {UserId}", userId);
                return Internal($"Error while generating invite link: {ex.Message}");
            }
        }

        public async Task<IResult> GetGroupByInviteCode(string inviteCode, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Fetching invite info for code: {InviteCode}", inviteCode);

                (long ownerId, string? groupNumber) = GroupLinkGenerator.DecryptInviteCode(inviteCode);
                if (groupNumber == null)
                {
                    _logger.LogWarning("Invalid invite code: {InviteCode}", inviteCode);
                    return InvalidArgument("Invalid invite code");
                }


                var group = await _groupRepo.GetGroup(groupNumber, ct);
                if (group == null)
                {
                    _logger.LogWarning("Group {GroupNumber} not found for invite code", groupNumber);
                    return NotFound("Group not found");
                }

                var headmanResult = await _userService.GetUser(ownerId, ct);
                string headmanName = "Unknown";
                if (headmanResult.IsSuccess && headmanResult.Value != null)
                {
                    var headman = headmanResult.Value;
                    headmanName = $"{headman.FirstName} {headman.LastName}".Trim();
                }
                else
                {
                    _logger.LogWarning("Headman {OwnerId} not found for group {GroupNumber}",
                        ownerId, groupNumber);
                }

                var membersCount = await _userRepo.GetStudentCount(groupNumber, ct);

                _logger.LogInformation("Successfully fetched invite info for group {GroupNumber}", groupNumber);

                return Ok(group).WithField("groupName", group.Name)
                    .WithField("groupNumber", group.Number)
                    .WithField("headmanName", headmanName)
                .WithField("membersCount", membersCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching invite info for code {InviteCode}", inviteCode);
                return Internal($"Error while fetching invite info: {ex.Message}");
            }
        }

        public async Task<IResult> JoinGroupByInviteCode(long userId, string inviteCode, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("User {UserId} attempting to join group with invite code", userId);

                (long ownerId, string? groupNumber) = GroupLinkGenerator.DecryptInviteCode(inviteCode);
                if (groupNumber == null)
                {
                    _logger.LogWarning("Invalid invite code: {InviteCode}", inviteCode);
                    return InvalidArgument("Invalid invite code");
                }


                var userResult = await _userService.GetUser(userId, ct);
                if (userResult.IsFailure)
                {
                    _logger.LogWarning("User {UserId} not found when joining group", userId);
                    return userResult;
                }

                var user = userResult.Value;

                if (!string.IsNullOrEmpty(user.GroupNumber))
                {
                    _logger.LogWarning("User {UserId} is already in group {GroupNumber}",
                        userId, user.GroupNumber);
                    return AlreadyExists("User is already a member of another group");
                }

                if (!string.IsNullOrEmpty(user.HeadmanOf))
                {
                    _logger.LogWarning("User {UserId} is headman of group {HeadmanOf}",
                        userId, user.HeadmanOf);
                    return AlreadyExists("Headman cannot join another group");
                }

                var group = await _groupRepo.GetGroup(groupNumber, ct);
                if (group == null)
                {
                    _logger.LogWarning("Group {GroupNumber} not found when user {UserId} tried to join",
                        groupNumber, userId);
                    return NotFound("Group not found");
                }

                user.GroupNumber = groupNumber;

                var updateResult = await _userService.UpdateUser(user, ct);

                if (updateResult.IsFailure)
                {
                    _logger.LogError("Failed to add user {UserId} to group {GroupNumber}",
                        userId, groupNumber);
                    return updateResult;
                }

                _logger.LogInformation("User {UserId} successfully joined group {GroupNumber}",
                    userId, groupNumber);


                return Ok().WithField("redirectTo", "/group");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while user {UserId} joining group with code {InviteCode}",
                    userId, inviteCode);
                return Internal($"Error while joining group: {ex.Message}");
            }
        }

        public async Task<IResult> LeaveGroup(long userId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("User {UserId} attempting to leave the group", userId);

                var userResult = await _userService.GetUser(userId, ct);
                if (userResult.IsFailure)
                {
                    _logger.LogWarning("User {UserId} not found when joining group", userId);
                    return userResult;
                }

                var user = userResult.Value;

                if (string.IsNullOrEmpty(user.GroupNumber))
                {
                    _logger.LogWarning("User {UserId} does not participate in any group",
                        userId);
                    return NotFound("User does not participate in any group");
                }

                if (!string.IsNullOrEmpty(user.HeadmanOf))
                {
                    _logger.LogWarning("User {UserId} is a headman of group {HeadmanOf}",
                        userId, user.HeadmanOf);
                    return Conflict("Headman cannot leave group");
                }

                user.GroupNumber = null;

                var updateResult = await _userService.UpdateUser(user, ct);

                if (updateResult.IsFailure)
                {
                    _logger.LogError("Failed to leave group by user {UserId}",
                        userId);
                    return updateResult;
                }

                _logger.LogInformation("User {UserId} successfully leave group",
                    userId);


                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while user {UserId} leaving group",
                    userId);
                return Internal($"Error while leaving group: {ex.Message}");
            }
        }

        public async Task<IResult> RemoveMember(long headmanId, long memberId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("User {UserId} attempting to remove group member {MemberId}", headmanId, memberId);

                if (headmanId == memberId)
                {
                    _logger.LogWarning("User {UserId} attempting to remove himself", headmanId);


                    /////////////////////////////////////////////

                    return InvalidArgument("Самый умный или что? Не нужно тут лазить");

                    /////////////////////////////////////////////


                }

                var headmanResult = await _userService.GetUser(headmanId, ct);
                if (headmanResult.IsFailure)
                {
                    _logger.LogWarning("Headman user {UserId} not found", headmanId);
                    return headmanResult;
                }

                var headman = headmanResult.Value!;

                var memberResult = await _userService.GetUser(memberId, ct);

                if (memberResult.IsFailure)
                {
                    _logger.LogWarning("User {UserId} not found", memberId);
                    return memberResult;
                }

                if (memberResult.Value!.GroupNumber != headman.HeadmanOf)
                {
                    _logger.LogWarning("User {MemberId} is not a member of user {HeadmanId} group", headmanId, memberId);
                    return Forbidden($"User {memberId} is not a member of user {headmanId} group");
                }

                var user = memberResult.Value;

                user.GroupNumber = null;

                var updateResult = await _userService.UpdateUser(user, ct);

                if (updateResult.IsFailure)
                {
                    _logger.LogError("Failed to remove user {UserId} from group",
                        user.Id);
                    return updateResult;
                }

                _logger.LogInformation("User {UserId} successfully removed from group",
                    user.Id);


                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while attempting remove user {UserId} from group",
                    memberId);
                return Internal($"Error while removing user from group: {ex.Message}");
            }
        }

        public async Task<IResult> DeleteGroup(long headmanId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("User {CallerId} attempting to delete group",
                    headmanId);

                var userResult = await _userService.GetUser(headmanId, ct);

                if (userResult.IsFailure)
                {
                    _logger.LogWarning("User {CallerId} not found when deleting group", headmanId);
                    return userResult;
                }

                if (userResult.Value!.HeadmanOf == null)
                {
                    _logger.LogWarning("User {CallerId} does not moderate any group",
                        headmanId);
                    return Unauthorized("Only headman can delete group");
                }

                var groupNumber = userResult.Value.HeadmanOf;

                var groupResult = await _groupRepo.GetGroup(groupNumber, ct);

                if (groupResult == null)
                {
                    _logger.LogWarning("Group {GroupNumber} not found when deleting by user {CallerId}",
                        groupNumber, headmanId);

                    return NotFound("Group not gound");
                }

                _groupRepo.DeleteGroup(groupResult);

                var deleted = await _groupRepo.SaveChangesAsync(ct);

                if (!deleted)
                {
                    _logger.LogError("Failed to delete group {GroupNumber} by user {CallerId}",
                        groupNumber, headmanId);
                    return Internal("Failed to delete group");
                }

                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting group by user {CallerId}", headmanId);
                return Internal($"Error while deleting group: {ex.Message}");
            }
        }

        public async Task<IResult> TransferOwnership(long headmanId, long newHeadmanId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("User {UserId} is attempting to transfer group ownership to member {NewHeadmanId}",
                        headmanId, newHeadmanId);

                if (headmanId == newHeadmanId)
                {
                    _logger.LogInformation("User {UserId} is attempting to transfer group ownership to yourself",
                           headmanId);
                    return InvalidArgument("It is not possible to transfer group owhership to yourself");
                }

                var headmanResult = await _userService.GetUser(headmanId, ct);
                if (headmanResult.IsFailure)
                {
                    _logger.LogWarning("Headman user {UserId} not found", headmanId);
                    return headmanResult;
                }

                var headman = headmanResult.Value!;

                if (string.IsNullOrEmpty(headman.HeadmanOf) ||
                    string.IsNullOrEmpty(headman.GroupNumber))
                {
                    _logger.LogWarning("User {UserId} is not a headman", headmanId);
                    return Forbidden("You are not a headman of the group");
                }

                var memberResult = await _userService.GetUser(newHeadmanId, ct);

                if (memberResult.IsFailure)
                {
                    _logger.LogWarning("User {UserId} not found", newHeadmanId);
                    return memberResult;
                }

                if (memberResult.Value!.GroupNumber != headman.HeadmanOf)
                {
                    _logger.LogWarning("User {MemberId} is not a member of user {HeadmanId} group", headmanId, newHeadmanId);
                    return Forbidden($"User {newHeadmanId} is not a member of user {headmanId} group");
                }

                var user = memberResult.Value;

                string groupNumber = headman.HeadmanOf!;
                headman.HeadmanOf = null;


                var updateResult = await _userService.UpdateUser(headman, ct);

                if (updateResult.IsFailure)
                {
                    _logger.LogError("Failed to update HeadmanOf filed for user {UserId}",
                        headmanId);
                    return updateResult;
                }

                var transferResult = await _userService.MakeHeadmanOf(newHeadmanId, groupNumber, ct);
                if (transferResult.IsFailure)
                {
                    _logger.LogError("Error while updating headman for group {GroupId}",
                        groupNumber);

                    await _userService.MakeHeadmanOf(headmanId, groupNumber, ct);


                    return transferResult;
                }

                _logger.LogInformation("Headman successfully transferred in group {GroupId}. Old: {OldHeadman}, New: {NewHeadman}",
                    groupNumber, headmanId, newHeadmanId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while transferring ownership from user {UserId} to user {NewHeadmanId}",
                    headmanId, newHeadmanId);

                return Internal($"Error while transferring group ownership");
            }
        }
        
        public async Task<IResult> Update(long callerId, UpdateGroupDto createGroupDto, CancellationToken ct)
        {
            try
            {
                var userResult = await _userService.GetUser(callerId, ct);
                if (userResult.IsFailure)
                {
                    return userResult;
                }

                if (userResult.Value!.HeadmanOf == null)
                {
                    return NotFound("You are not a headman");
                }


                return Ok();
            }
            catch (Exception ex)
            {
                return Internal($"Error occurred while updating group");
            }
        }
    }
}