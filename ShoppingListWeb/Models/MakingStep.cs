using System.ComponentModel.DataAnnotations;

namespace ShoppingListWeb.Models
{
    public class MakingStep
    {
        [Key]
        public int MakingStepId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}