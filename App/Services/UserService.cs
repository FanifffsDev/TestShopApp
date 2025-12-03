using AutoMapper;
using TestShopApp.Api.Interefaces;
using TestShopApp.App.Interfaces;
using TestShopApp.App.Models;
using TestShopApp.App.Models.User;
using TestShopApp.App.ReturnTypes;
using TestShopApp.App.Utils;
using TestShopApp.Domain.Base;
using TestShopApp.Telegram.Utils;
using static TestShopApp.App.ReturnTypes.ResultImport;
using IResult = TestShopApp.App.ReturnTypes.IResult;

namespace TestShopApp.App.Services
{
    public class UserService(IUserRepo userRepo, IMapper mapper, ILogger<UserService> logger) : IUserService
    {
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserService> _logger = logger;

        public async Task<IResult> RegisterUser(long userId, RegisterUserDto user, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Attempting to register user with ID: {UserId}", userId);

                var existingUser = await _userRepo.GetUser(userId, ct);

                if (existingUser != null)
                {
                    _logger.LogWarning("User with ID {UserId} already exists", userId);
                    return AlreadyExists("User already exists");
                }

                if (user.Role == "student")
                {
                    await _userRepo.AddUser(new User
                    {
                        Id = userId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = "student",
                        CreatedAt = DateTimeUtils.GetCurrentTimeFormatted(),
                        UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted(),

                    }, ct);
                }
                else if (user.Role == "teacher")
                {
                    await _userRepo.AddUser(new User
                    {
                        Id = userId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        ThirdName = user.ThirdName,
                        Subject = user.Subject,
                        Role = "teacher",
                        CreatedAt = DateTimeUtils.GetCurrentTimeFormatted(),
                        UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted()
                    }, ct);
                }
                else
                {
                    _logger.LogWarning("Invalid argument while registering user {UserId}", userId);
                    return InvalidArgument("Incorrect role");
                }

                var saved = await _userRepo.SaveChangesAsync(ct);

                if (!saved)
                {
                    _logger.LogError("Failed to save user {UserId} to database", userId);
                    return Internal("Failed to add user");
                }

                _logger.LogInformation("User {UserId} registered successfully with role {Role}", userId, user.Role);

                return Ok().WithField("redirectTo", "/profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user {UserId}", userId);
                return Internal(ex.Message);
            }
        }

        public async Task<IResult<UserDto?>> GetUser(long userId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Fetching user with ID: {UserId}", userId);

                var user = await _userRepo.GetUser(userId, ct);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return NotFound<UserDto?>("User not found");
                }

                var userDto = _mapper.Map<UserDto>(user);
                _logger.LogInformation("User {UserId} fetched successfully", userId);
                return Ok(userDto).WithField("user", userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user {UserId}", userId);
                return Internal<UserDto?>($"Error while fetching user: {ex.Message}");
            }
        }

        public async Task<IResult<UserDto>> MakeHeadmanOf(long userId, string groupNumber, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Attempting to make user {UserId} headman of group {GroupNumber}", userId, groupNumber);

                var user = await _userRepo.GetUser(userId, ct);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found when attempting to make headman", userId);
                    return NotFound<UserDto>("User not found");
                }

                if (user.Role != "student")
                {
                    _logger.LogWarning("User {UserId} with role {Role} cannot be headman", userId, user.Role);
                    return InvalidArgument<UserDto>("Only students can be headman");
                }

                var existingHeadman = await _userRepo.GetHeadmanByGroup(groupNumber, ct);
                if (existingHeadman != null && existingHeadman.Id != userId)
                {
                    _logger.LogWarning("Group {GroupNumber} already has headman with ID {ExistingHeadmanId}",
                        groupNumber, existingHeadman.Id);
                    return AlreadyExists<UserDto>($"Group {groupNumber} already has a headman");
                }

                user.HeadmanOf = groupNumber;
                user.GroupNumber = groupNumber;
                user.UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted();

                _userRepo.UpdateUser(user);
                var saved = await _userRepo.SaveChangesAsync(ct);

                if (!saved)
                {
                    _logger.LogError("Failed to save headman assignment for user {UserId}", userId);
                    return Internal<UserDto>("Failed to assign headman");
                }

                var userDto = _mapper.Map<UserDto>(user);
                _logger.LogInformation("User {UserId} successfully made headman of group {GroupNumber}", userId, groupNumber);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while making user {UserId} headman of group {GroupNumber}",
                    userId, groupNumber);
                return Internal<UserDto>($"Error while assigning headman: {ex.Message}");
            }
        }

        public async Task<IResult<UserDto?>> UpdateCommonUserData(AuthUser user, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Updating common data for user {UserId}", user.Id);

                var existingUser = await _userRepo.GetUser(user.Id, ct);

                if (existingUser == null)
                {
                    _logger.LogWarning("User {UserId} not found when updating common data", user.Id);
                    return NotFound<UserDto?>("User not found");
                }

                existingUser.PhotoUrl = user.PhotoUrl;
                existingUser.LanguageCode = user.LanguageCode;
                existingUser.Username = user.Username;
                existingUser.TgFirstName = user.FirstName;
                existingUser.TgLastName = user.LastName;
                existingUser.UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted();

                _userRepo.UpdateUser(existingUser);
                var saved = await _userRepo.SaveChangesAsync(ct);

                if (!saved)
                {
                    _logger.LogError("Failed to save common data for user {UserId}", user.Id);
                    return Internal<UserDto?>("Failed to update user data");
                }

                var userDto = _mapper.Map<UserDto>(existingUser);
                _logger.LogInformation("Common data updated successfully for user {UserId}", user.Id);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating common data for user {UserId}",
                    user?.Id ?? 0);
                return Internal<UserDto?>($"Error while updating user: {ex.Message}");
            }

        }

        public async Task<IResult<UserDto?>> UpdateUserProfile(long userId, UpdateUserProfileDto updateDto, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Updating user profile for user {UserId}", userId);

                var existingUser = await _userRepo.GetUser(userId, ct);

                if (existingUser == null)
                {
                    _logger.LogWarning("User {UserId} not found when updating profile", userId);
                    return NotFound<UserDto>("User does not exist");
                }

                _mapper.Map(updateDto, existingUser);
                existingUser.Id = userId;
                existingUser.UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted();

                if (existingUser.Role == "student")
                {
                    existingUser.FirstName = updateDto.FirstName;
                    existingUser.LastName = updateDto.LastName;
                }
                else if (existingUser.Role == "teacher")
                {
                    existingUser.FirstName = updateDto.FirstName;
                    existingUser.LastName = updateDto.LastName;
                    existingUser.ThirdName = updateDto.ThirdName;
                    existingUser.Subject = updateDto.Subject;
                }

                _userRepo.UpdateUser(existingUser);

                var saved = await _userRepo.SaveChangesAsync(ct);

                if (!saved)
                {
                    _logger.LogError("Failed to save updated profile for user {UserId}", userId);
                    return Internal<UserDto>("Failed to update user");
                }

                var userDto = _mapper.Map<UserDto>(existingUser);
                _logger.LogInformation("User profile updated successfully for user {UserId}", userId);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user profile {UserId}", userId);
                return Internal<UserDto>("Error while updating the user: " + ex.Message);
            }
        }

        public async Task<IResult<UserDto?>> UpdateUser(UserDto updateDto, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Updating user {UserId}", updateDto.Id);

                var existingUser = await _userRepo.GetUser(updateDto.Id, ct);

                if (existingUser == null)
                {
                    _logger.LogWarning("User {UserId} not found when updating it", updateDto.Id);
                    return NotFound<UserDto>("User does not exist");
                } 

                _userRepo.UpdateUser(_mapper.Map<User>(updateDto));

                var saved = await _userRepo.SaveChangesAsync(ct);

                if (!saved)
                {
                    _logger.LogError("Failed to save updated user {UserId}", updateDto.Id);
                    return Internal<UserDto>("Failed to update user");
                }

                var userDto = _mapper.Map<UserDto>(existingUser);
                _logger.LogInformation("User profile updated successfully for user {UserId}", updateDto.Id);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user profile {UserId}", updateDto.Id);
                return Internal<UserDto>("Error while updating the user: " + ex.Message);
            }
        }

        public async Task<IResult> BakeUser(TelegramInitDataRawDto data, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Baking user with Telegram init data");

                (bool isVerified, AuthUser authUser) = TgAuthUtils.VerifyInitData(data.InitData);

                if (!isVerified || authUser == null)
                {
                    _logger.LogWarning("Failed to verify Telegram init data");
                    return Unauthorized("Invalid token data");
                }

                _logger.LogInformation("Telegram init data verified for user {UserId}", authUser.Id);

                var updateUserRes = await UpdateCommonUserData(authUser, ct);
                var user = await GetUser(authUser.Id, ct);

                string token = "tma " + TgAuthUtils.GenerateExtendedInitData(authUser, updateUserRes.ErrorType != ErrorType.NotFound);

                _logger.LogInformation("User {UserId} baked successfully, isRegistered: {IsRegistered}",
                    authUser.Id, updateUserRes.IsSuccess);

                return Ok()
                    .WithField("isRegistered", updateUserRes.IsSuccess)
                    .WithFieldIf("hasGroup", user.Value?.GroupNumber != null, user.IsSuccess)
                    .WithHeader("Authorization", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while baking user");
                return Internal("Error while processing user authentication: " + ex.Message);
            }
        }
    }
}