using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    public class Customer
    {
        public string CustomerId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;        
        public string Phone { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Barangay { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

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
