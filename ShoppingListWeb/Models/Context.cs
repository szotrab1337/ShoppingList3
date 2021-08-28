using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShoppingListWeb.Models
{
    internal class Context : GenericContext
    {
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Unit> Units{ get; set; }
        public DbSet<User> Users{ get; set; }

        public Context() : base("ShoppingList")
        {
        }
    }
}