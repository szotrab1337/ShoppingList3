using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingListWeb.Models
{
    public class Shop
    {
        [Key]
        public int ShopId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        [NotMapped]
        public string LastModified => !ModifiedOn.HasValue ? CreatedOn.ToString(@"dd.MM.yyyy HH:mm:ss") : ModifiedOn.Value.ToString(@"dd.MM.yyyy HH:mm:ss");

        public virtual List<Item> Items { get; set; }
    }
}
