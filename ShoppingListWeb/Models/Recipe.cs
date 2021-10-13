using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ShoppingListWeb.Models
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
        public virtual List<MakingStep> MakingSteps { get; set; }

        [NotMapped]
        public double TimeOfMakingTheRecipeMinutes => TimeSpan.FromTicks(TimeOfMakingTheRecipe).TotalMinutes;

        [NotMapped]
        public string FormattedTimeOfMakingTheRecipe => TimeSpan.FromTicks(TimeOfMakingTheRecipe).TotalMinutes.ToString() + " min";

        [NotMapped]
        public Image Picture => !string.IsNullOrWhiteSpace(PictureRaw) ? PictureConverter.Base64ToImage(PictureRaw) : null;

        [NotMapped]
        public string PictureFormatted => String.Format("data:image/png;base64,{0}", PictureRaw);
    }
}