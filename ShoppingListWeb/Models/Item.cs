using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        [NotMapped]
        public List<Unit> AvailableUnits
        {
            get
            {
                List<Unit> availableUnits = new List<Unit>();
                using (Context context = new Context())
                {
                    availableUnits = context.Units.Where(x => x.ForShopping.Value).ToList();
                }

                return  availableUnits;
            }
        }

        public string ValidateItem()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "Nazwa przedmiotu nie może być pusta.";

            if (Quantity.HasValue && Quantity.Value <= 0)
                return "Ilość nie może być mniejsza lub równa zero.";

            return string.Empty;
        }

        public virtual Unit Unit { get; set; }
        public virtual Shop Shop{ get; set; }
    }
}
