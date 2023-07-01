using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace WarehouseManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //This function will be called on startup of the applications
        protected override void OnStartup(StartupEventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            if (principal.IsInRole(WindowsBuiltInRole.Administrator) == false && principal.IsInRole(WindowsBuiltInRole.User) == true)
            {
                ProcessStartInfo objProcessInfo = new ProcessStartInfo();
                objProcessInfo.UseShellExecute = true;
                objProcessInfo.FileName = Assembly.GetEntryAssembly().CodeBase;
                objProcessInfo.UseShellExecute = true;
                objProcessInfo.Verb = "Warehouse Management";
                try
                {
                    Process proc = Process.Start(objProcessInfo);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
