using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WarehouseManagement.Models
{
    public class MenuItem
    {
        public string Header { get; set; }
        public List<SubMenuItem> SubMenuItems { get; set; }
        public MenuItem(string header, List<SubMenuItem> subMenuItems)
        {
            Header = header;
            this.SubMenuItems = subMenuItems;
        }
    }
}
