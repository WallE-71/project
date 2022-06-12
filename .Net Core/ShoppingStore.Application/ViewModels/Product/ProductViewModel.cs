using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Entities;

namespace ShoppingStore.Application.ViewModels.Product
{
    public class ProductViewModel : BaseViewModel<string>
    {
        public ProductViewModel()
        {
        }
        public ProductViewModel(string id)
        {
            Id = id;
        }
        public ProductViewModel(JsonResult result, string id)
        {
            Id = id;
            Result = new JsonResult(result);
        }

        [Display(Name = "تصویر محصول")]
        public string ImageName { get; set; }

        [Display(Name = "تصویر محصول"), Required(ErrorMessage = "انتخاب {0} الزامی است."), JsonIgnore]
        public string ImageFile { get; set; }
        public ICollection<string> ImageFiles { get; set; }

        [Display(Name = "موجودی"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public int Stock { get; set; }

        [Display(Name = "قیمت"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public int Price { get; set; }
        public string DisplayPrice { get; set; }

        [Display(Name = "تاریخ عرضه"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), JsonIgnore]
        public string PersianInsertDate { get; set; }

        [Display(Name = "تاریخ انقضا")]
        public string PersianExpirationDate { get; set; }
        public System.DateTime? ExpirationDate { get; set; }

        [Display(Name = "وزن")]
        public double Weight { get; set; }

        [Display(Name = "اندازه")]
        public string Size { get; set; }

        [Display(Name = "دسته"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string NameOfCategories { get; set; }

        public string NameOfSeller { get; set; }

        [JsonIgnore]
        public double BestOfSellers { get; set; }

        [Display(Name = "وضعیت")]
        public string DisplayStates { get; set; }
        public ProductState States { get; set; }

        [Display(Name = "تولید شده در")]
        public string MadeIn { get; set; }

        [Display(Name = "تخفیف")]
        public string DisplayDiscount { get; set; }
        public double PercentDiscount { get; set; }    

        [JsonIgnore]
        public Discount Discount { get; set; }
      
        [Display(Name = "محاسبه تخفیف")]
        public string ActionDiscount { get; set; }
                 
        [Display(Name = "محصول پیشنهادی")]
        public bool IsPrefered { get; set; }

        [Display(Name = "تعداد فروخته شده")]
        public int? NumberOfSale { get; set; }

        [Display(Name = "NumberOfVisit")]
        public int NumberOfVisit { get; set; }

        [Display(Name = "NumberOfLike"), JsonIgnore]
        public int NumberOfLike { get; set; }

        [Display(Name = "NumberOfDisLike"), JsonIgnore]
        public int NumberOfDisLike { get; set; }

        [Display(Name = "NumberOfComments")]
        public int NumberOfComments { get; set; }

        [Display(Name = "ShortName")]
        public string ShortName { get; set; }

        [Display(Name = "بارکد")]
        public byte[] Barcode { get; set; }

        [JsonPropertyName("مواد / ترکیبات"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string NameOfMaterial { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; }

        [Display(Name = "رنگ اصلی")]
        public string PrimaryColor { get; set; }

        [JsonPropertyName("colors")]
        public string NameOfColor { get; set; }

        [JsonIgnore]
        public List<string> ColorsName { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProductColor> ProductColors { get; set; }

        [JsonPropertyName("برند"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string NameOfBrand { get; set; }

        [JsonIgnore]
        public int BrandId { get; set; }

        [JsonPropertyName("ضمانت/گارانتی"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string NameOfGuarantee { get; set; }

        [JsonIgnore]
        public int GuaranteeId { get; set; }

        [JsonIgnore]
        public bool IsBookmarked { get; set; }

        [JsonIgnore]
        public int[] CategoryIds { get; set; }

        [JsonIgnore]
        public int IdOfCategories { get; set; }

        [JsonIgnore]
        public List<int> CategoryIdsList { get; set; }

        [JsonIgnore]
        public ProductCategoriesViewModel ProductCategoriesViewModel { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }     

        [JsonIgnore]
        public int? SellerId { get; set; }

        [JsonIgnore]
        public bool? IsSeller { get; set; }
        
        [JsonIgnore]
        public long[] SellerProductIds { get; set; }

        /////////////////////////
        [JsonIgnore]
        public JsonResult Result { get; set; }

        [JsonIgnore]
        public bool CheckStatus { get; set; }

        [JsonIgnore]
        public bool CheckCategories { get; set; }
        
        [JsonIgnore]
        public bool IsPresent { get; set; }

        [JsonIgnore]
        public bool IsEdit { get; set; } = false;

        [JsonIgnore]
        public bool IsTransparent { get; set; }
    }
}
