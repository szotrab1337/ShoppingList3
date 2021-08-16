using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingList.Models
{
    public static class UnitBase
    {
        private static readonly List<Unit> Units = new List<Unit>()
        {
            new Unit(){ UnitId = 0, Name = "sztuki", ShortName = "szt" },
            new Unit(){ UnitId = 1, Name = "litry", ShortName = "l" },
            new Unit(){ UnitId = 2, Name = "opakowania", ShortName = "op" },
            new Unit(){ UnitId = 3, Name = "kilogramy", ShortName = "kg" },
            new Unit(){ UnitId = 4, Name = "gramy", ShortName = "g" }
        };

        public static List<Unit> GetUnits()
        {
            return Units;
        }

        public static string GetUnit(int unitId)
        {
            return Units.FirstOrDefault(x => x.UnitId == unitId).ShortName;
        }
    }
}
