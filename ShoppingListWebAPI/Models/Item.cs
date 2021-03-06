using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShoppingListWebAPI.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; }
        public double? Quantity { get; set; }
        public string Description { get; set; }
        public int? UnitId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
