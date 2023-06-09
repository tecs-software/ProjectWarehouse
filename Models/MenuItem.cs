using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WarehouseManagement.Models
{
    internal class MenuItem
    {
        public string Header { get; set; }
        public List<SubMenuItem> SubMenuItems { get; set; }
        public MenuItem(string header, List<SubMenuItem> subMenItems)
        {
            Header = header;
            this.SubMenuItems = subMenItems;
        }
    }
}
