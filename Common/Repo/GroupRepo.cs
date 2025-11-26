using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TestShopApp.Common.Data;
using Group = TestShopApp.Common.Data.Group;

namespace TestShopApp.Common.Repo
{
    public class GroupRepo(AppDbContext context) : IGroupRepo
    {
        private readonly AppDbContext _context = context;

        public async Task<ExecutionResult<Group>> CreateGroup(Group group)
        {
            try
            {
                if (await GroupExists(group.Number))               
                    return new ExecutionResult<Group>(false, null, "Group already exists");
                
                await _context.Groups.AddAsync(group);

                return new ExecutionResult<Group>(await SaveChangesAsync(), group);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Ошибка при создании группы {Number}", group.Number);
                return new ExecutionResult<Group>(false, null, "Error while creating a group");
            }
        }

        private async Task<bool> GroupExists(string number)
        {
            return await _context.Groups
                .AnyAsync(g => g.Number == number);
        }

        public async Task<ExecutionResult<Group>> UpdateGroup(string groupNumber, string groupName, long callerId)
        {
            throw new NotImplementedException();
        }

        public async Task<ExecutionResult<Group>> DeleteGroup(string groupNumber, long callerId)
        {
            throw new NotImplementedException();
        }








        public async Task<ExecutionResult<Group>> GetGroup(string groupNumber)
        {
            var group = await _context.Groups.AsNoTracking().FirstOrDefaultAsync(g => g.Number == groupNumber);

            if (group == null)
            {
                return new ExecutionResult<Group>(false, null, "Group doesn't exists");
            }

            return new ExecutionResult<Group>(true, group);
        }

        public async Task<ExecutionResult<IEnumerable<User>>> GetGroupMembers(string groupNumber)
        {
            var students = await _context.Users
                .Where(u => u.GroupNumber == groupNumber && u.Role == "student")
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            if (students == null)
            {
                return new ExecutionResult<IEnumerable<User>>(false, null, "Group doesn't contain any members");
            }

            return new ExecutionResult<IEnumerable<User>>(true, students);
        }

        public async Task<ExecutionResult> AddStudentToGroup(long studentId, string groupNumber)
        {
            var student = await _context.Users.FindAsync(studentId);

            if (student == null || student.Role != "student")
            {
                return new ExecutionResult(false, "Student doesn't exists");
            }

            student.GroupNumber = groupNumber;

            return new ExecutionResult(await SaveChangesAsync());
        }

        public async Task<ExecutionResult> RemoveStudentFromGroup(long studentId)
        {
            var student = await _context.Users.FindAsync(studentId);

            if (student == null)
            {
                return new ExecutionResult(false, "Student doesn't exists");
            }

            student.GroupNumber = null;

            return new ExecutionResult(await SaveChangesAsync());
        }

        public async Task<ExecutionResult<int>> GetStudentCount(string groupNumber)
        {
            var res = await _context.Users.CountAsync(u => u.GroupNumber == groupNumber && u.Role == "student");

            return new ExecutionResult<int>(true, res);
        }

        private async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
