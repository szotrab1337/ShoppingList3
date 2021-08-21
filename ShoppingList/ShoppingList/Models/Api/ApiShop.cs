using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingList.Models.Api
{
    public class ApiShop
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? Number { get; set; }

        public virtual List<ApiItem> Items {  get; set; }

        public string LastModifiedOn => !ModifiedOn.HasValue ? CreatedOn.ToString(@"dd.MM.yyyy HH:mm:ss") : ModifiedOn.Value.ToString(@"dd.MM.yyyy HH:mm:ss");
        public DateTime LastModifiedOnDate => !ModifiedOn.HasValue ? CreatedOn : ModifiedOn.Value;
    }
}
