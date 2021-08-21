using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingList.Models.Api
{
    public class ApiItem
    {
        public int ItemId { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; }
        public double? Quantity { get; set; }
        public string Description { get; set; }
        public int? UnitId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual ApiShop Shop {  get; set; }
    }
}
