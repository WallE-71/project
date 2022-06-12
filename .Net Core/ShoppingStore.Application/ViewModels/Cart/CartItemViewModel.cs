using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShoppingStore.Application.ViewModels.Cart
{
    public class CartItemViewModel : BaseViewModel<int>
    {
        public int Price { get; set; }
        public int Count { get; set; }
        public string Image { get; set; }
        public string ProductName { get; set; }
        public List<string> Colors { get; set; }
        public string ProductId { get; set; }

        [JsonIgnore]
        public string NameOfColor { get; set; }

        public string SelectedColor { get; set; }
    }
}
