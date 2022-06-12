using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Application.ViewModels.CreditCart
{
    public class CreditCartViewModel
    {
        public int Row { get; set; }

        [Display(Name = "CreditCart")]
        public Domain.Entities.CreditCart CreditCart { get; set; }
        
        [Display(Name = "اعتبار"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), Range(80000, 9999999, ErrorMessage = "رقم اعتبار باید از ۸۰۰۰۰ تا ۹۹۹۹۹۹۹ باشد")]
        public int Credit { get; set; }
        public string stringCredit { get; set; }

        //[NationalIdValidate("/",@"\"," ")]
        [Display(Name = "کد ملی"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string NationalId { get; set; }

        [Display(Name = "شماره شبا")]
        public string BankCode { get; set; }

        public string Owner { get; set; }
        public string PersianInsertTime { get; set; }
        public DateTime? InsertTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; }
    }
}
