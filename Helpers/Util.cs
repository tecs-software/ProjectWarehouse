using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WarehouseManagement.Models;
using MenuItem = System.Windows.Controls.MenuItem;

namespace WarehouseManagement.Helpers
{
    internal class Util
    {
        public static readonly string status_in_stock = "IN-STOCK";
        public static readonly string status_out_of_stock = "OUT OF STOCK";
        public static readonly string status_low_stock = "LOW-STOCK";
        public static readonly string status_discontinued = "DISCONTINUED";
        public static readonly string status_in_progress = "IN PROGRESS";
        public static readonly string status_voided = "VOIDED";
        public static readonly string status_completed = "COMPLETED";


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

        public static bool IsAnyStringEmpty(params string[] strings)
        {
            foreach (var str in strings)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAnyComboBoxItemEmpty(ComboBox comboBox)
        {
            foreach (var item in comboBox.Items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAnyStringEmpty(params object[] strings)
        {
            foreach (var obj in strings)
            {
                if (obj is string str && string.IsNullOrWhiteSpace(str))
                {
                    return true;
                }
                else if (obj is SecureString secureStr && IsSecureStringEmpty(secureStr))
                {
                    return true;
                }
            }
            return false;
        }


        public static bool IsSecureStringEmpty(SecureString secureString)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToBSTR(secureString);
                return Marshal.PtrToStringBSTR(ptr) == string.Empty;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ptr);
                }
            }
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

        public static bool SecurePasswordsMatch(SecureString securePassword1, SecureString securePassword2)
        {
            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;

            try
            {
                // Convert the SecureString to a BSTR
                bstr1 = Marshal.SecureStringToBSTR(securePassword1);
                bstr2 = Marshal.SecureStringToBSTR(securePassword2);

                // Convert the BSTRs to strings
                string str1 = Marshal.PtrToStringBSTR(bstr1);
                string str2 = Marshal.PtrToStringBSTR(bstr2);

                // Compare the strings using the String.Equals method
                return string.Equals(str1, str2);
            }
            finally
            {
                // Free the BSTRs
                if (bstr1 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr1);
                }

                if (bstr2 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr2);
                }
            }
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void ClearTextBoxes(params TextBox[] textBoxes)
        {
            foreach (TextBox textBox in textBoxes)
            {
                textBox.Text = string.Empty;
            }
        }

        public static void HideToolTip(TextBox textBox)
        {
            var toolTip = textBox.ToolTip as ToolTip;
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
            }
        }

        public static decimal SumTextBoxes(params object[] values)
        {
            decimal sum = 0;
            foreach (object value in values)
            {
                decimal decimalValue = 0;
                if (value is string stringValue)
                {
                    decimal.TryParse(stringValue, out decimalValue);
                }
                else if (value is decimal decimalNumber)
                {
                    decimalValue = decimalNumber;
                }
                sum += decimalValue;
            }
            return sum;
        }

        public static string calculateTextBox(TextBox textBox1, char operation, TextBox textBox2)
        {
            decimal num1 = 0, num2 = 0, num3 = 0;

            if (textBox1 != null && decimal.TryParse(textBox1.Text.Replace(",", ""), out decimal x))
            {
                num1 = x;
            }

            if (textBox2 != null && decimal.TryParse(textBox2.Text.Replace(",", ""), out decimal y))
            {
                num2 = y;
            }

            switch (operation)
            {
                case '*':
                    num3 = num1 * num2;
                    break;
                case '/':
                    if (num2 != 0)
                    {
                        num3 = num1 / num2;
                    }
                    break;
                case '+':
                    num3 = num1 + num2;
                    break;
                case '-':
                    num3 = num1 - num2;
                    break;
                default:
                    break;
            }

            return num3.ToString("N2");
        }

        public static void ValidateIfProfitable(TextBox textBox)
        {
            if (decimal.TryParse(textBox.Text.Replace(",", ""), out decimal result) && result > 0)
            {
                textBox.BorderBrush = Brushes.Green;
            }
            else if (decimal.TryParse(textBox.Text.Replace(",", ""), out decimal result2) && result <= 0)
            {
                textBox.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox.BorderBrush = Brushes.Black;
            }
        }

        public static void setTextBoxToolTip(TextBox textBox, string content)
        {
            var toolTip = new ToolTip();
            toolTip.Content = content;
            toolTip.Placement = PlacementMode.Relative;
            toolTip.PlacementTarget = textBox;
            toolTip.HorizontalOffset = textBox.ActualWidth + 10;
            textBox.ToolTip = toolTip;
            toolTip.IsOpen = true;
        }

        public static void ShowContextMenuForButton(Button? button, params MenuItem[] menuItems)
        {
            if (button == null)
                return;

            ContextMenu menu = new ContextMenu();
            foreach (MenuItem item in menuItems)
            {
                menu.Items.Add(item);
            }

            button.ContextMenu = menu;
            button.ContextMenu.IsOpen = true;
        }

        public static void ShowContextMenuForControl(Control control, params MenuItem[] menuItems)
        {
            ContextMenu menu = new ContextMenu();
            menu.Background = Brushes.WhiteSmoke;
            menu.FontSize = 14;
            menu.Margin = new Thickness(0);

            foreach (MenuItem item in menuItems)
            {
                item.Background = Brushes.WhiteSmoke;
                item.FontSize = 14;
                menu.Items.Add(item);
            }

            control.ContextMenu = menu;
            control.ContextMenu.IsOpen = true;
        }

        public static async Task<(List<Address.Province>, List<Address.Municipality>, List<Address.Barangay>)> LoadAddressData()
        {
            List<Address.Province>? provinces = new();
            List<Address.Municipality>? municipalities = new();
            List<Address.Barangay>? barangays = new();

            await Task.Run(() =>
            {
                string provinceJson = File.ReadAllText("Components/table_province.json");
                provinces = JsonConvert.DeserializeObject<List<Address.Province>>(provinceJson);

                string municipalityJson = File.ReadAllText("Components/table_municipality.json");
                municipalities = JsonConvert.DeserializeObject<List<Address.Municipality>>(municipalityJson);

                string barangayJson = File.ReadAllText("Components/table_barangay.json");
                barangays = JsonConvert.DeserializeObject<List<Address.Barangay>>(barangayJson);

                if (provinces != null && municipalities != null && barangays != null)
                {
                    provinces = provinces.OrderBy(p => p.province_name).ToList();
                    municipalities = municipalities.OrderBy(m => m.municipality_name).ToList();
                    barangays = barangays.OrderBy(b => b.barangay_name).ToList();
                }
            });

            return (provinces, municipalities, barangays);
        }

        public static string GetLocalSqlServerInstanceName()
        {
            string instanceName = string.Empty;
            string serverName = Environment.MachineName;

            ServiceController[] services = ServiceController.GetServices().Where(s => s.ServiceName.StartsWith("MSSQL$")).ToArray();

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
                catch { }
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
