using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingList.Models
{
    public class Unit : BaseModel
    {
        public int UnitId { get; set; }
        public string Name { get; set; }
    }
}
