using System.Data.Entity;

namespace ShoppingListWebAPI.Models
{
    internal class Context : GenericContext
    {
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<MakingStep> MakingSteps { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        public Context() : base("ShoppingList")
        {
        }
    }
}