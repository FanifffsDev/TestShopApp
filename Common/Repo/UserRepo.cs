using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestShopApp.App.Models.User;
using TestShopApp.Common.Data;
using TestShopApp.Common.Utils;

namespace TestShopApp.Common.Repo
{
    public class UserRepo(AppDbContext context) : IUserRepo
    {
        private readonly AppDbContext _context = context;
        public async Task<ExecutionResult<User>> AddUser(User user)
        {
            await _context.AddAsync(user);
            return new ExecutionResult<User>(await SaveChangesAsync(), user);
        }

        public async Task<ExecutionResult<User>> GetUser(long userId)
        {
            var res = await _context.Users.FindAsync(userId);
            
            return new ExecutionResult<User>(res != null, res);
        }

        public async Task<ExecutionResult<User>> UpdateUser(long userId, UpdateUserDto updateDto, IMapper mapper)
        {
            try
            {
                var existingUser = await _context.Users
                    .AsNoTracking() 
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (existingUser == null)
                {
                    return new ExecutionResult<User>(false, null, "User does not exists");
                }

                mapper.Map(updateDto, existingUser);
                existingUser.Id = userId;

                if (existingUser.Role == "student")
                {
                    _context.Users.Attach(existingUser);

                    _context.Entry(existingUser).Property(x => x.FirstName).IsModified = true;
                    _context.Entry(existingUser).Property(x => x.LastName).IsModified = true;
                    //_context.Entry(existingUser).Property(x => x.Group).IsModified = true;
                }
                else if(existingUser.Role == "teacher")
                {
                    _context.Users.Attach(existingUser);

                    _context.Entry(existingUser).Property(x => x.FirstName).IsModified = true;
                    _context.Entry(existingUser).Property(x => x.LastName).IsModified = true;
                    _context.Entry(existingUser).Property(x => x.ThirdName).IsModified = true;
                    _context.Entry(existingUser).Property(x => x.Subject).IsModified = true;
                }

                existingUser.UpdatedAt = DateTimeUtils.GetCurrentTimeFormatted();


                return new ExecutionResult<User>(await SaveChangesAsync(), existingUser);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ошибка при обновлении пользователя {UserId}", userId);
                return new ExecutionResult<User>(false, null, "Error while updating data");
            }
        }

        public Task<ExecutionResult<User>> MakeHeadmanOf(long userId, string groupNumber)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
