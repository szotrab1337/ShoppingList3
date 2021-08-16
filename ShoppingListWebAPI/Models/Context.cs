using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShoppingListWebAPI.Models
{
    internal class Context : GenericContext
    {
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Item> Items { get; set; }

        public Context() : base("ShoppingList")
        {
        }
    }
}