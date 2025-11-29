using Microsoft.EntityFrameworkCore;
using TestShopApp.App.Interfaces;
using TestShopApp.Domain.Base;

namespace TestShopApp.Infrastructure.Repo
{
    public class UserRepo(AppDbContext context) : IUserRepo
    {
        private readonly AppDbContext _context = context;

        public async Task AddUser(User user, CancellationToken ct) => await _context.AddAsync(user, ct);

        public async Task<User?> GetUser(long userId, CancellationToken ct) => await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);

        public void UpdateUser(User updatedUser) => _context.Users.Update(updatedUser);

        public async Task<bool> SaveChangesAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct) > 0;

        public async Task<User?> GetHeadmanByGroup(string groupNumber, CancellationToken ct) => await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.HeadmanOf == groupNumber, ct);

        public async Task<IEnumerable<User?>> GetGroupMembers(string groupNumber, CancellationToken ct)
        {
            return await _context.Users
                .Where(u => u.GroupNumber == groupNumber)
                .OrderBy(u => u.LastName)
                .OrderByDescending(u => u.HeadmanOf == groupNumber)
                .ThenBy(u => u.FirstName)
                .ToListAsync(ct);
        }

        public async Task<int> GetStudentCount(string groupNumber, CancellationToken ct) => await _context.Users.CountAsync(u => u.GroupNumber == groupNumber);
    }
}
