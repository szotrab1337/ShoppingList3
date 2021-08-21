using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShoppingListWeb.Models
{
    public class Unit
    {
        [Key]
        public int UnitId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}
