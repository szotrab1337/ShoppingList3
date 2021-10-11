using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingListWebAPI.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public double? Quantity{ get; set; }
        public int? UnitId { get; set; }
        public int Number { get; set; }
    }
}