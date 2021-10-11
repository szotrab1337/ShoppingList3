using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingListWebAPI.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public string Hints { get; set; }
        public long TimeOfMakingTheRecipe { get; set; }
        public double NumberOfServings { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PictureRaw { get; set; }
        public string Source { get; set; }

        public virtual List<Ingredient> Ingredients { get; set; }
        public virtual List<MakingStep> MakingSteps {  get; set; }
    }
}