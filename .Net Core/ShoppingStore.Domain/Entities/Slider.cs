using System.Collections.Generic;

namespace ShoppingStore.Domain.Entities
{
    public class Slider : BaseEntity<int>
    {
        public string Url { get; set; }
        public TypeOfSlider TypeOfSlider { get; set; }
        public ImageLocation ImageLocation { get; set; }
        public virtual ICollection<FileStore> FileStores { get; set; }
    }

    public enum TypeOfSlider
    {
        Static = 1,
        Dynamic = 2,
        Ads = 3
    }

    public enum ImageLocation
    {
        Top = 1,
        Center = 2,
        Bottom = 3,
    }
}
