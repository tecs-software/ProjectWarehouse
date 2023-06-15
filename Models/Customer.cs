using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    internal class Customer
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Barangay { get; set; }
        public string Address { get; set; }

        public void SetData(string[] propertyNames, string[] propertyValues)
        {
            Type customerType = typeof(Customer);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                PropertyInfo property = customerType.GetProperty(propertyNames[i]);
                if (property != null)
                {
                    property.SetValue(this, propertyValues[i]);
                }
            }
        }
    }
}
