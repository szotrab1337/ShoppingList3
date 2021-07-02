using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingList.Models
{
    public class Shop
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int Number { get; set; }
    }
}
