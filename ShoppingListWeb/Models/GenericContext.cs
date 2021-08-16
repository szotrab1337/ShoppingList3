using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShoppingListWeb.Models
{
    internal class GenericContext : DbContext
    {
        public GenericContext(string ConnStr) : base(ConnStr)
        {
            Database.CommandTimeout = 10;
        }
    }
}