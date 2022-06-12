using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Application.ViewModels.Category
{
    public class CategoryViewModel : BaseViewModel<int>
    {
        [JsonIgnore]
        public int? ParentId { get; set; }

        [Display(Name = "دسته پدر")]
        public string ParentName { get; set; }

        public virtual ICollection<CategoryViewModel> SubCategories { get; set; }
    }
}
