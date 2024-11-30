using Microsoft.EntityFrameworkCore;
using NoventiqApplication.Services;

namespace NoventiqApplication.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Roles.AnyAsync(r => r.Name == name);
        }

        public async Task<bool> ExistsByDescriptionAsync(string description)
        {
            return await _context.Roles.AnyAsync(r => r.Description == description);
        }
    }
}
