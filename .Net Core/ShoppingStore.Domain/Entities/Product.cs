using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Domain.Entities
{
    public class Product : BaseEntity<string>
    {
        public int Stock { get; set; }
        public int Price { get; set; }
        public ProductState States { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public double Weight { get; set; }
        public string Size { get; set; }
        public byte[] Barcode { get; set; }
        public string MadeIn { get; set; }
        public bool IsPrefered { get; set; }
        public int? NumberOfSale { get; set; }

        public virtual int BrandId { get; set; }
        public virtual int? SellerId { get; set; }
        public virtual int? GuaranteeId { get; set; }
        public virtual ICollection<Visit> Visits { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Bookmark> Bookmarks { get; set; }
        public virtual ICollection<FileStore> FileStores { get; set; }
        public virtual ICollection<ProductColor> ProductColors { get; set; }
        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }

    public enum ProductState
    {
        [Display(Name = "دمو")]
        Demo = 1,

        [Display(Name = "در انبار")]
        Stockroom = 2,

        [Display(Name = "آماده عرضه")]
        Ready = 3,

        [Display(Name = "موجود نیست")]
        EndOfStock = 4,

        [Display(Name = "منقضی شده")]
        ExpirationEnd = 5,

        [Display(Name = "مرجوع شده")]
        Returned = 6,

        [Display(Name = "آسیب دیده")]
        Corrupted = 7,

        [Display(Name = "بزودی")]
        CoomingSoon = 8,
    }   
}
