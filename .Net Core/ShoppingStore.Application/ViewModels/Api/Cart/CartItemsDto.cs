using System.Collections.Generic;

namespace ShoppingStore.Application.ViewModels.Api.CartItem
{
    public class CartItemDto
    {
        public int Price { get; set; }
        public int Count { get; set; }
        public string Image { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }       
        public Domain.Entities.Product Product { get; set; }
        public virtual List<string> Colors { get; set; }
    }
}
