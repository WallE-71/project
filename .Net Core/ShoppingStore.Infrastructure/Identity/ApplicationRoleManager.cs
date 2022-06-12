using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ShoppingStore.Domain.Common.Extensions;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Infrastructure.Contracts;
using ShoppingStore.Application.ViewModels.RoleManager;

namespace ShoppingStore.Infrastructure.Identity
{
    public class ApplicationRoleManager : RoleManager<Role>, IApplicationRoleManager
    {
        private readonly IRoleStore<Role> _store;
        private readonly IdentityErrorDescriber _errors;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly IApplicationUserManager _userManager;
        private readonly ILogger<ApplicationRoleManager> _logger;
        private readonly IEnumerable<IRoleValidator<Role>> _roleValidators;
        public ApplicationRoleManager(
            IRoleStore<Role> store,
            ILookupNormalizer keyNormalizer,
            ILogger<ApplicationRoleManager> logger,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            IdentityErrorDescriber errors,
            IApplicationUserManager userManager) :
            base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _errors = errors;
            _errors.CheckArgumentIsNull(nameof(_errors));
            _keyNormalizer = keyNormalizer;
            _keyNormalizer.CheckArgumentIsNull(nameof(_keyNormalizer));
            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));
            _store = store;
            _store.CheckArgumentIsNull(nameof(_store));
            _roleValidators = roleValidators;
            _roleValidators.CheckArgumentIsNull(nameof(_roleValidators));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
        }

        public List<Role> GetAllRoles()
        {
            return Roles.ToList();
        }

        public Task<Role> FindClaimsInRole(int roleId)
        {
            return Roles.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == roleId);
        }

        public async Task<List<RolesViewModel>> GetPaginateRolesAsync(int offset, int limit, bool? roleNameSortAsc, string searchText)
        {
            List<RolesViewModel> roles;
            roles = await Roles.Where(r => r.Name.Contains(searchText)).Select(role => new RolesViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                UsersCount = role.UserRoles.Count()
            }).Skip(offset).Take(limit).ToListAsync();

            if (roleNameSortAsc != null)
                roles = roles.OrderBy(t => (roleNameSortAsc == true && roleNameSortAsc != null) ? t.Name : "").OrderByDescending(t => (roleNameSortAsc == false && roleNameSortAsc != null) ? t.Name : "").ToList();

            foreach (var item in roles)
                item.Row = ++offset;
            return roles;
        }
    }
}
