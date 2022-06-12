using System.Collections.Generic;

namespace ShoppingStore.Application.ViewModels.Api.Cart
{
    public class CartDto : BaseViewModel<int>
    {  
        public int UserId { get; set; }
        public int SumAmount { get; set; }
        public int ProductCount { get; set; }
        public virtual List<CartItem.CartItemDto> CartItems { get; set; }
    }    
}
