using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Application.ViewModels.City
{
    public class CityProvinceViewModel
    {       
        public CityProvinceViewModel(){}
        public CityProvinceViewModel(int cityId)
        {
            CityId = cityId;
        }

        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("ردیف")]
        public int Row { get; set; }
        public int CityId { get; set; }

        [Display(Name = "شهر"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string CityName { get; set; }

        [Display(Name = "استان"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string ProvinceName { get; set; }

        public int numberOfUser { get; set; }     
        public int numberOfCities { get; set; }
    }
}
