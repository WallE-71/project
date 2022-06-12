using System.Collections.Generic;
using ShoppingStore.Domain.Entities.Identity;

namespace ShoppingStore.Application.ViewModels.UserManager
{
    public class ListOfRolesViewModel
    {
        public ListOfRolesViewModel(List<Role> roles, int[] roleIds)
        {
            Roles = roles;
            RoleIds = roleIds;
        }

        public int[] RoleIds { get; set; }
        public List<Role> Roles { get; set; }
    }
}
