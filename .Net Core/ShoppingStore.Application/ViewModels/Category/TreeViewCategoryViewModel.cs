using System.Collections.Generic;

namespace ShoppingStore.Application.ViewModels.Category
{
    public class TreeViewCategory
    {
        public TreeViewCategory()
        {
            subs = new List<TreeViewCategory>();
        }

        public int id { get; set; }
        public string title { get; set; }
        public List<TreeViewCategory> subs { get; set; }
    }
}
