using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShoppingListWeb.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public double? Quantity { get; set; }
        public int? UnitId { get; set; }
        public int Number { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual Unit Unit { get; set; }

        [NotMapped]
        public string NameShort => $"{Name} {Unit.ShortName}";

        [NotMapped]
        public string QuantityFormatted => Quantity.HasValue ? Quantity.ToString() + " " + Unit.ShortName : string.Empty;

        [NotMapped]
        public List<Unit> AvailableUnits
        {
            get
            {
                List<Unit> availableUnits = new List<Unit>();
                using (Context context = new Context())
                {
                    availableUnits = context.Units.ToList();
                }

                return availableUnits;
            }
        }

        public string ValidateItem()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "Nazwa składnika nie może być pusta.";

            if (Quantity.HasValue && Quantity.Value <= 0)
                return "Ilość nie może być mniejsza lub równa zero.";

            return string.Empty;
        }
    }
}