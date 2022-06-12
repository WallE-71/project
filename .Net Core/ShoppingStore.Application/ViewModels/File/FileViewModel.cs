using System.Text.Json.Serialization;

namespace ShoppingStore.Application.ViewModels.File
{
    public class FileViewModel
    {
        [JsonPropertyName("ردیف")]
        public int Row { get; set; }

        [JsonPropertyName("تصویر")]
        public string ImageName { get; set; }

        [JsonPropertyName("فایل/سند")]
        public string FileName { get; set; }

        [JsonPropertyName("نام محصول")]
        public virtual string ProductName { get; set; }

        [JsonPropertyName("نام فروشنده")]
        public virtual string SellerName { get; set; }
    }
}
