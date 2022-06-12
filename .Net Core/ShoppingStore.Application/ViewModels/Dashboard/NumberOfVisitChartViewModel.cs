using System.Text.Json.Serialization;

namespace ShoppingStore.Application.ViewModels.Dashboard
{
    public class NumberOfVisitChartViewModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("y")]
        public int Value { get; set; }
    }
}
