using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingListWebAPI.Models
{
    public class Shop
    {
        [Key]
        public int ShopId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual List<Item> Items { get; set; }
    }
}
