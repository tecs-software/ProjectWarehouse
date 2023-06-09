using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    internal class SubMenuItem
    {
        public string Name { get; set; }
        public int Numbers { get; set; }


        public SubMenuItem(string name, int numbers)
        {
            Name = name;
            Numbers = numbers;
        }

    }
}
