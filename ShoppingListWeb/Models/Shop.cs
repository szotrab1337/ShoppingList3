using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingListWeb.Models
{
    public class Shop
    {
        [Key]
        public int ShopId { get; set; }
        public string Name { get; set; }

        public virtual List<Item> Items { get; set; }
    }
}
