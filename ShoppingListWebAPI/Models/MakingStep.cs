using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingListWebAPI.Models
{
    public class MakingStep
    {
        [Key]
        public int MakingStepId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}