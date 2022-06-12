using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ShoppingStore.Domain.Entities.Identity
{
    public class Role : IdentityRole<int>
    {
        public Role() { }
        public Role(string name) : base(name) { }
        public Role(string name, string description) : base(name)
        {
            Description = description;
        }

        public string Description { get; set; }
        public DateTime? RemoveTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? InsertTime { get; set; }

        public virtual ICollection<RoleClaim> Claims { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

    }
}
