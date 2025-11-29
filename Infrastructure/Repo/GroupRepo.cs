using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TestShopApp.App.Interfaces;
using TestShopApp.App.Utils;
using TestShopApp.Domain.Base;
using TestShopApp.Telegram;
using Group = TestShopApp.Domain.Base.Group;

namespace TestShopApp.Infrastructure.Repo
{
    public class GroupRepo(AppDbContext context) : IGroupRepo
    {
        private readonly AppDbContext _context = context;

        public async Task CreateGroup(Group group, CancellationToken ct) => await _context.Groups.AddAsync(group, ct);
        
        public async Task<Group?> GetGroup(string groupNumber, CancellationToken ct) => await _context.Groups.AsNoTracking().FirstOrDefaultAsync(g => g.Number == groupNumber, cancellationToken: ct);

        public void UpdateGroup(Group group) => _context.Groups.Update(group);

        public void DeleteGroup(Group group) => _context.Groups.Remove(group);


        public async Task<bool> SaveChangesAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct) > 0;
    }
}
