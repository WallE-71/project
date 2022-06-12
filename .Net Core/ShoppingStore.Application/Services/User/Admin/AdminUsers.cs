using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;

namespace ShoppingStore.Application.Services
{
    public class AdminUsers : IAdminUsers
    {
        private readonly IDataBaseContext _context;
        public AdminUsers(IDataBaseContext context)
        {
            _context = context;
        }

        public async Task<string> GetUserImageAsync(int? userId)
        {
            var img = await (from user in _context.Users.Include(u => u.FileStores)
                             where (user.FileStores.Count() != 0)
                             select new
                             {
                                 Image = user.FileStores.FirstOrDefault(f => f.UserId == userId).ImageName
                             }).AsNoTracking().FirstOrDefaultAsync();
            if (img == null) return null;
            return img.Image;
        }

        public async Task<int[]> GetRoleIdsAsync(int? userId)
        {
            var ids = await (from user in _context.Users
                             join ur in _context.UserRoles on user.Id equals ur.UserId into nc
                             from userRole in nc.DefaultIfEmpty()
                             where (user.Id == userId)
                             select new
                             {
                                 IdOfRoles = userRole != null ? userRole.RoleId : 0,
                             }).ToListAsync();

            return ids.Select(n => n.IdOfRoles).Distinct().ToArray();
        }       
    }
}
