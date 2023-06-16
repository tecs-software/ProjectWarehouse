using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WarehouseManagement.Models
{
    internal class CurrentUser
    {
        public int? userID { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? accessLevel { get; set; }

        private static CurrentUser? instance;

        private CurrentUser() { }

        public static CurrentUser Instance
        {
            get
            {
                if (instance == null)
                    instance = new CurrentUser();
                return instance;
            }
        }

        public void Clear()
        {
            userID = 0;
            firstName = null;
            middleName = null;
            lastName = null;
            accessLevel = null;
        }
    }
}
