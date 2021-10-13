using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
}