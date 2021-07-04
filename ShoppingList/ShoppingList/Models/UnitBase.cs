using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingList.Models
{
    public static class UnitBase
    {
        private static readonly Dictionary<int, string> Units = new Dictionary<int, string>()
        {
            { 0, "sztuki" },
            { 1, "litry" },
            { 2, "opakowania" },
            { 3, "kilogramy" }
        };

        public static List<Unit> GetUnits()
        {
            List<Unit> units = new List<Unit>();

            foreach (var unit in Units)
            {
                units.Add(new Unit { UnitId = unit.Key, Name = unit.Value });
            }

            return units;
        }
    }
}
