using System.Collections.Generic;

namespace ShoppingStore.Application.ViewModels.Cart
{
    public class CartViewModel : BaseViewModel<int>
    {
        public int UserId { get; set; }
        public int SumAmount { get; set; }
        public string Customer { get; set; }
        public int ProductCount { get; set; }
        public virtual List<CartItemViewModel> CartItems { get; set; }
    }
}
