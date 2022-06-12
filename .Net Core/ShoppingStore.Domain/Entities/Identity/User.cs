using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ShoppingStore.Domain.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public string Bio { get; set; }
        public bool IsActive { get; set; }
        public bool? IsValidAccount { get; set; }
        public DateTime? RemoveTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? InsertTime { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<UserRole> Roles { get; set; }
        public virtual ICollection<UserClaim> Claims { get; set; }
        public virtual ICollection<FileStore> FileStores { get; set; }
    }

    public enum GenderType
    {
        [Display(Name = "مرد")]
        Male = 1,

        [Display(Name = "زن")]
        Female = 2
    }
}
