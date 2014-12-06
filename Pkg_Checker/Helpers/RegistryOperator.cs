using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Helpers
{
    public class RegistryOperator
    {
        public static bool WriteRegistry(String appName, String keyEID, String valEID, String keyPWD, String valPWD)
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software", true);
                registryKey = registryKey.CreateSubKey(appName);
                registryKey.SetValue(keyEID, valEID, RegistryValueKind.String);
                registryKey.SetValue(keyPWD, valPWD, RegistryValueKind.String);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ReadRegistry(String appName, String keyEID, out String valEID, String keyPWD, out String valPWD)
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software", true);
                registryKey = registryKey.OpenSubKey(appName);
                valEID = registryKey.GetValue(keyEID, String.Empty).ToString();
                valPWD = registryKey.GetValue(keyPWD, String.Empty).ToString();
                return true;
            }
            catch
            {
                valEID = String.Empty;
                valPWD = String.Empty;
                return false;
            }
        }
    }
}
