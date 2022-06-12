using System;

namespace ShoppingStore.Domain.Entities
{
    public class Newsletter : BaseEntity<int>
    {
        public Newsletter() { }
        public Newsletter(string email)
        {
            Email = email;
        }
        
        public string Email { get; set; }
    }
}
