using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    internal class Roles
    {
        public int roleID { get; set; }
        public string roleName { get; set; }
        public decimal hourlyRate { get; set; }

        public void SetData(string[] propertyNames, string[] propertyValues)
        {
            Type roleType = typeof(Roles);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                PropertyInfo property = roleType.GetProperty(propertyNames[i]);
                if (property != null)
                {
                    property.SetValue(this, propertyValues[i]);
                }
            }
        }
    }
}
