using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WarehouseManagement.Helpers
{
    internal class Util
    {
        public static bool IsAnyTextBoxEmpty(params Control[] controls)
        {
            foreach (var control in controls)
            {
                if (control is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text.Trim()))
                {
                    return true;
                }
                else if (control is PasswordBox passwordBox && string.IsNullOrWhiteSpace(passwordBox.Password.Trim()))
                {
                    return true;
                }
            }
            return false;
        }

        public static string HashPassword(SecureString password)
        {
            IntPtr bstr = IntPtr.Zero;

            try
            {
                bstr = Marshal.SecureStringToBSTR(password);
                int length = Marshal.ReadInt32(bstr, -4);
                byte[] bytes = new byte[length];
                Marshal.Copy(bstr, bytes, 0, length);

                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] hashBytes = sha256Hash.ComputeHash(bytes);
                    StringBuilder builder = new StringBuilder();

                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        builder.Append(hashBytes[i].ToString("x2"));
                    }

                    return builder.ToString().Substring(0, Math.Min(builder.Length, 60));
                }
            }
            finally
            {
                if (bstr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr);
                }
            }
        }

        public static string GetLocalSqlServerInstanceName()
        {
            string instanceName = string.Empty;
            string serverName = Environment.MachineName;

            // Get all SQL Server services installed on the machine
            ServiceController[] services = ServiceController.GetServices().Where(s => s.ServiceName.StartsWith("MSSQL$")).ToArray();

            // Check each service to see if it's running, and get its instance name
            foreach (ServiceController service in services)
            {
                try
                {
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        instanceName = service.ServiceName.Replace("MSSQL$", "");
                        break;
                    }
                }
                catch { } // Ignore any services we can't access
            }

            if (instanceName == string.Empty)
            {
                if(serverName == string.Empty)
                {
                    return "No Open Connection";
                }
                return $"{serverName}";
            }
            return $"{serverName}\\{instanceName}";
        }
    }
}
