using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Models
{
    internal class User
    {
        public int userID { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public string authenticationCode { get; set; }
        public string username { get; set; }
        public SecureString password { get; set; }

        public void SetData(string[] propertyNames, string[] propertyValues)
        {
            Type userType = typeof(User);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                PropertyInfo property = userType.GetProperty(propertyNames[i]);
                if (property != null)
                {
                    property.SetValue(this, propertyValues[i]);
                }
            }
        }

    }
}
