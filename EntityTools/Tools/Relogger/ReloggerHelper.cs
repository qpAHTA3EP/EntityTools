using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace EntityTools.Tools.Relogger
{
    public static class ReloggerHelper
    {
        public static string GetMachineIdFromRegistry()
        {
            using (var crypticCoreKey = Registry.CurrentUser.OpenSubKey(@"Software\Cryptic\Core"))
            {
                if (crypticCoreKey != null)
                {
                    var machineId = crypticCoreKey.GetValue("machineId");
                    return machineId?.ToString() ?? string.Empty;
                }
            }

            return string.Empty;
#if false
            using (var crypticLauncherKey = Registry.CurrentUser.OpenSubKey(@"Software\Cryptic\Cryptic Launcher"))
            {
                if (crypticLauncherKey != null)
                {
                    var userName = crypticLauncherKey.GetValue("UserName");
                    lblAccount.Text = userName.ToString();
                }
            } 
#endif
        }

        
    }
}
