using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ShoppingListWeb.Models
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

        [NotMapped]
        public string LastModified => !ModifiedOn.HasValue ? CreatedOn.ToString(@"dd.MM.yyyy HH:mm:ss") : ModifiedOn.Value.ToString(@"dd.MM.yyyy HH:mm:ss");

        [NotMapped]
        public string QuantityFormatted => Quantity.HasValue ? Quantity.Value.ToString() + " " + Unit.ShortName : "---";

        [NotMapped]
        public string DescriptionFormatted => string.IsNullOrEmpty(Description) ? "---" : Description;

        public virtual Unit Unit { get; set; }
    }
}
